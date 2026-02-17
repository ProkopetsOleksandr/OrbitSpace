# Roadmap: Реализация аутентификации

Задачи разбиты на **эпики** — логические блоки, каждый из которых заканчивается рабочим, проверяемым результатом. Внутри каждого эпика задачи выполняются **строго по порядку** — каждая следующая опирается на предыдущую.

---

## Эпик 1 — База данных: фундамент аутентификации

**Цель:** Создать схему БД, на которой держится вся система токенов.

**Задачи:**

1. Добавить в таблицу `users` поля для аутентификации:
   `password_hash`, `email_verified`, `is_active`, `failed_login_attempts`,
   `locked_until`, `tokens_valid_after`
2. Создать таблицу `refresh_tokens` с полями:
   `id`, `token_hash`, `user_id`, `family_id`, `device_id`, `created_at`,
   `expires_at`, `is_revoked`, `revoked_at`, `revoked_reason`, `replaced_by`,
   `absolute_expires_at`
3. Создать таблицу `email_verification_tokens`:
   `id`, `user_id`, `token_hash`, `expires_at`, `is_used`
4. Добавить индексы:
   - `refresh_tokens.token_hash` (уникальный, для поиска по токену)
   - `refresh_tokens.user_id` (для logout all devices)
   - `refresh_tokens.family_id` (для revoke всей семьи)
   - Partial index `WHERE is_revoked = FALSE` на `expires_at`
5. Создать и применить EF Core migration

**✓ Результат:** Схема БД готова, migration применена, таблицы видны в DBeaver/pgAdmin.

---

## Эпик 2 — .NET: доменный слой и инфраструктура

**Цель:** Подготовить сущности и интерфейсы, чтобы бизнес-логика
не зависела от деталей реализации.

**Задачи:**

1. Создать `User` domain entity с нужными полями и методами
   (`VerifyEmail()`, `IncrementFailedLogin()`, `LockAccount()`)
2. Создать `RefreshToken` domain entity с методами `Revoke(reason)`, `IsExpired()`
3. Определить интерфейсы в Application слое:
   - `IUserRepository` — `FindByEmail`, `FindById`, `Create`, `Update`
   - `IRefreshTokenRepository` — `FindByHash`, `FindByFamilyId`,
     `Create`, `RevokeFamily`, `RevokeAllForUser`
   - `IPasswordHasher` — `Hash(password)`, `Verify(password, hash)`
   - `ITokenService` — `GenerateAccessToken(user)`, `GenerateRefreshToken()`
4. Реализовать `IPasswordHasher` в Infrastructure через `Konscious.Security.Cryptography.Argon2`:
   - Параметры: memory=19456 KiB, iterations=2, parallelism=1
   - Генерировать случайный salt при каждом хэшировании
5. Реализовать репозитории через EF Core в Infrastructure слое
6. Зарегистрировать всё в DI-контейнере

**✓ Результат:** Код компилируется, unit-тесты на `PasswordHasher.Hash + Verify` проходят.

---

## Эпик 3 — .NET: регистрация

**Цель:** Endpoint `POST /api/auth/register` создаёт пользователя и возвращает профиль.

**Задачи:**

1. Создать `RegisterCommand` (CQRS, MediatR) с полями `Email`, `Password`,
   `FirstName`, `LastName`
2. Добавить валидацию через FluentValidation:
   - Email формат и уникальность (запрос к репозиторию)
   - Пароль: минимум 8 символов, максимум 128
3. В `RegisterCommandHandler`:
   - Захэшировать пароль через `IPasswordHasher`
   - Создать `User` entity
   - Сохранить через `IUserRepository`
   - Сгенерировать токен верификации email (crypto-random, 32 байта)
   - Сохранить SHA-256 хэш токена в `email_verification_tokens`
4. Создать `AuthController` с `POST /api/auth/register`
5. Настроить FluentValidation pipeline behaviour в MediatR
6. Вернуть `201 Created` с `{ id, email, message: "Check your email" }`

**✓ Результат:** `POST /api/auth/register` через Postman создаёт пользователя в БД.

---

## Эпик 4 — .NET: логин и генерация токенов

**Цель:** Endpoint `POST /api/auth/login` возвращает пару токенов.

**Задачи:**

1. Реализовать `ITokenService`:
   - `GenerateAccessToken(user)`:
     - Алгоритм RS256 (сгенерировать RSA key pair, приватный ключ — в User Secrets)
     - Claims: `sub`, `email`, `role`, `jti` (новый Guid), `iat`, `exp` (+15 мин)
     - Claims: `iss` (URL API), `aud` (идентификатор клиента)
     - Использовать `JsonWebTokenHandler` из `Microsoft.IdentityModel.JsonWebTokens`
   - `GenerateRefreshToken()`:
     - `RandomNumberGenerator.GetBytes(32)` → base64url строка
     - Вернуть raw token (для клиента) + `SHA256(raw)` (для хранения в БД)
2. Создать `LoginCommand` с `Email`, `Password`, `DeviceId`
3. В `LoginCommandHandler`:
   - Найти пользователя по email
   - Если не найден — вернуть ту же ошибку что и для неверного пароля
     (защита от user enumeration)
   - Верифицировать пароль через `IPasswordHasher.Verify()`
   - Проверить `email_verified` и `is_active`
   - Сгенерировать access token + refresh token
   - Создать запись `RefreshToken` в БД:
     новый `family_id` (Guid), `device_id` из запроса, `expires_at = +7 дней`
   - Вернуть `{ accessToken, refreshToken, user: { id, email, firstName } }`
4. Настроить валидацию JWT входящих запросов:
   `AddAuthentication().AddJwtBearer(...)` в `Program.cs`
   с `TokenValidationParameters`: `ValidAlgorithms = ["RS256"]`,
   `ClockSkew = TimeSpan.FromSeconds(30)`, `MapInboundClaims = false`
5. Добавить rate limiting: 5 запросов в минуту per IP на `/api/auth/login`

**✓ Результат:** `POST /api/auth/login` возвращает `accessToken` и `refreshToken`.
Защищённый endpoint `GET /api/me` с `[Authorize]` возвращает профиль при передаче токена.

---

## Эпик 5 — .NET: ротация refresh-токенов

**Цель:** Endpoint `POST /api/auth/refresh` реализует RTR с детекцией кражи.

**Задачи:**

1. Создать `RefreshTokenCommand` с полем `RefreshToken` (raw token из куки)
2. В `RefreshTokenCommandHandler` реализовать полный алгоритм:
   - Вычислить `hash = SHA256(rawToken)`
   - Найти запись в БД по `token_hash`
   - Если не найден → `401 invalid_token`
   - Если `is_revoked = TRUE`:
     → **Reuse detected**: вызвать `RevokeFamily(family_id)`,
     залогировать инцидент, вернуть `401 reuse_detected`
   - Если `expires_at < NOW()` → `401 token_expired`
   - Открыть транзакцию:
     - Пометить старый токен: `is_revoked = TRUE`, `revoked_reason = "rotation"`,
       `replaced_by = <new_token_id>`
     - Создать новый токен с тем же `family_id` и `device_id`
   - Зафиксировать транзакцию
   - Сгенерировать новый access token
   - Вернуть `{ accessToken, refreshToken }`
3. Добавить grace period: хранить `replaced_by`, при reuse в течение 30 секунд
   после ротации — вернуть ту же новую пару (идемпотентность)
4. Настроить rate limiting: 20 запросов в минуту per user на `/api/auth/refresh`

**✓ Результат:** `POST /api/auth/refresh` возвращает новую пару.
Повторное использование старого refresh token ревоцирует всю семью.

---

## Эпик 6 — .NET: logout

**Цель:** Пользователь может выйти с одного или всех устройств.

**Задачи:**

1. Создать `LogoutCommand` с `RefreshToken` (для single-device logout)
2. `LogoutCommandHandler`:
   - Найти токен по `SHA256(rawToken)`
   - Пометить всю семью (`family_id`) как `is_revoked = TRUE`,
     `revoked_reason = "logout"`
3. Создать `LogoutAllCommand` (только `user_id` из JWT claims, без body)
4. `LogoutAllCommandHandler`:
   - `RevokeAllForUser(userId)` — пометить все активные токены
   - Обновить `users.tokens_valid_after = NOW()`
5. Добавить проверку `tokens_valid_after` в JWT validation pipeline:
   через `JwtBearerEvents.OnTokenValidated` — если `iat < tokens_valid_after` → 401

**✓ Результат:** `POST /api/auth/logout` инвалидирует сессию.
После logout старый refresh token не работает. `POST /api/auth/logout-all` разлогинивает все устройства.

---

## Эпик 7 — .NET: фоновая очистка токенов

**Цель:** БД не засоряется устаревшими записями.

**Задачи:**

1. Создать `TokenCleanupService` как `BackgroundService`
2. Каждые 6 часов выполнять:
   ```sql
   DELETE FROM refresh_tokens
   WHERE (expires_at < NOW() - INTERVAL '7 days')
      OR (is_revoked = TRUE AND revoked_at < NOW() - INTERVAL '24 hours')
   ```
3. Логировать количество удалённых записей
4. Зарегистрировать сервис в `Program.cs`

**✓ Результат:** Сервис запускается вместе с API, периодически чистит таблицу.

---

## Эпик 8 — Next.js: BFF-фундамент (куки и прокси)

**Цель:** Слой BFF умеет хранить токены в httpOnly-куках и проксировать запросы.

**Задачи:**

1. Создать утилиту `lib/cookies.ts` с функциями:
   - `setAuthCookies(accessToken, refreshToken)` — записывает обе куки:
     access: `httpOnly, Secure, SameSite=Lax, maxAge=900, path=/`
     refresh: `httpOnly, Secure, SameSite=Lax, maxAge=604800, path=/api/auth/refresh`
   - `clearAuthCookies()` — удаляет обе куки
   - `getAccessToken()` — читает из `cookies()` (async в Next.js 16)
   - `getRefreshToken()` — читает из `cookies()`
2. Создать catch-all прокси `app/api/proxy/[...path]/route.ts`:
   - Читает access token из куки
   - Если нет токена → `401`
   - Добавляет `Authorization: Bearer <token>` к запросу
   - Проксирует на `DOTNET_API_URL` с сохранением метода, body, query params
   - Возвращает ответ клиенту
3. Добавить в `.env.local`: `DOTNET_API_URL=http://localhost:5000`

**✓ Результат:** Запрос через `fetch('/api/proxy/users/me')` из браузера
проходит через прокси с токеном и получает ответ от .NET API.

---

## Эпик 9 — Next.js: формы регистрации и логина

**Цель:** Пользователь может зарегистрироваться и войти через UI.

**Задачи:**

1. Создать схемы валидации Zod:
   `lib/schemas/auth.ts` — `registerSchema`, `loginSchema`
2. Создать Route Handler `app/api/auth/register/route.ts`:
   - Валидировать body через Zod
   - Проксировать на `.NET POST /api/auth/register`
   - Вернуть ответ клиенту (без токенов — их ещё нет)
3. Создать Route Handler `app/api/auth/login/route.ts`:
   - Валидировать body через Zod
   - Проксировать на `.NET POST /api/auth/login`
   - При успехе: вызвать `setAuthCookies(accessToken, refreshToken)`
   - Вернуть клиенту только `{ user: { id, email, firstName } }`
   - При ошибке: прокинуть статус и сообщение
4. Создать Client Component `RegisterForm` с React Hook Form + zodResolver
5. Создать Client Component `LoginForm` с React Hook Form + zodResolver:
   - При успехе: `router.push('/dashboard')` + `router.refresh()`
6. Создать страницы `/register` и `/login`

**✓ Результат:** Можно зарегистрироваться и войти через UI.
После логина в DevTools → Application → Cookies видны две httpOnly-куки.

---

## Эпик 10 — Next.js: DAL и защищённые Server Components

**Цель:** Server Components получают данные с авторизацией без передачи токена в браузер.

**Задачи:**

1. Создать `lib/dal.ts` (Data Access Layer):
   - `getSession()` через `React.cache()`:
     читает access token, если нет — `redirect('/login')`
   - `fetchFromApi<T>(endpoint, options?)`:
     добавляет `Authorization: Bearer` и делает fetch к `DOTNET_API_URL`
     при `401` — `redirect('/login')`
2. Создать Server Component `app/dashboard/page.tsx`:
   - Вызывает `fetchFromApi('/api/users/me')` для получения профиля
   - Вызывает `prefetchQuery` для TanStack Query (подготовка к Эпику 12)
3. Создать `components/UserMenu.tsx` (Server Component):
   использует `getSession()` для отображения имени пользователя
4. Убедиться, что `React.cache()` предотвращает дублирующиеся auth-запросы

**✓ Результат:** `/dashboard` рендерится на сервере с данными пользователя.
В Network DevTools нет запросов с токенами от браузера.

---

## Эпик 11 — Next.js: автоматическое обновление токенов

**Цель:** Пользователь никогда не видит 401, токены обновляются незаметно.

**Задачи:**

1. Создать Route Handler `app/api/auth/refresh/route.ts`:
   - Читает refresh token из куки
   - Вызывает `.NET POST /api/auth/refresh`
   - При успехе: `setAuthCookies(newAccess, newRefresh)`
   - При ошибке: `clearAuthCookies()`, вернуть `401`
2. **Proactive refresh** в `proxy.ts` (Next.js 16 middleware):
   - При каждом запросе на защищённый маршрут читать access token
   - Декодировать `exp` через `jose` (`decodeJwt()`, без верификации)
   - Если до истечения меньше 60 секунд — вызвать `/api/auth/refresh`
   - Если refresh не удался — `redirect('/login')`
3. **Reactive refresh** в catch-all прокси (из Эпика 8):
   - Если `.NET API` вернул `401`:
     - Singleton promise для предотвращения race condition
       (несколько одновременных запросов → один refresh)
     - Вызвать `fetch('/api/auth/refresh', { method: 'POST' })`
     - При успехе: retry исходного запроса с новым токеном из куки
     - При неудаче: вернуть `401` клиенту
4. Установить `jose`: `npm install jose`

**✓ Результат:** Если вручную установить короткое время жизни access token (1 мин),
пользователь продолжает работать без перебоев и без повторного логина.

---

## Эпик 12 — Next.js: logout и защита маршрутов

**Цель:** Пользователь может выйти, неаутентифицированный не попадает в приложение.

**Задачи:**

1. Создать Route Handler `app/api/auth/logout/route.ts`:
   - Читает refresh token из куки
   - Вызывает `.NET POST /api/auth/logout` с raw refresh token в body
   - Вызывает `clearAuthCookies()`
   - Возвращает `200 { success: true }`
2. Создать `LogoutButton` Client Component:
   - `useMutation` из TanStack Query → `POST /api/auth/logout`
   - При успехе: `queryClient.clear()` + `router.push('/login')` + `router.refresh()`
3. Настроить `proxy.ts` (middleware) для защиты маршрутов:
   - Публичные маршруты: `/login`, `/register`, `/api/auth/*`
   - На защищённых маршрутах: если нет access token И нет refresh token → `redirect('/login')`
   - Если нет access token НО есть refresh token → `redirect('/api/auth/refresh')`
     с `callbackUrl` для возврата после обновления

**✓ Результат:** Кнопка "Выйти" разлогинивает пользователя, куки удаляются.
Переход на `/dashboard` без авторизации редиректит на `/login`.
После logout старые куки не дают доступа.

---

## Итоговая карта зависимостей

```
Эпик 1 (БД)
  └── Эпик 2 (Domain + Infrastructure)
        ├── Эпик 3 (Регистрация)
        └── Эпик 4 (Логин + JWT)
              ├── Эпик 5 (Ротация токенов)
              ├── Эпик 6 (Logout)
              └── Эпик 7 (Cleanup)

Эпик 8 (BFF-фундамент)
  └── Эпик 9 (Формы)
        └── Эпик 10 (DAL + Server Components)
              └── Эпик 11 (Auto-refresh)
                    └── Эпик 12 (Logout + Route Protection)
```

.NET и Next.js части можно разрабатывать параллельно, начиная с Эпика 8,
пока .NET API тестируется через Postman.
