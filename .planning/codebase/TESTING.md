# Testing Patterns

**Analysis Date:** 2026-02-14

## Test Framework

**Status:** Not currently implemented

**Current state:**
- No test files present in `next-js-app/src/` or `dotnet-web-api/`
- No test runner configured (Jest, Vitest, xUnit not found)
- No test dependencies in `package.json`
- ESLint and TypeScript provide static analysis only

**Note:** The codebase is in active development and testing infrastructure is not yet established.

## Frontend Testing (Recommended Setup)

When testing is implemented, the frontend should use:

**Test Runner:** Vitest (modern, fast, TypeScript-first) or Jest

**Assertion Library:** Vitest built-in or Jest expect

**Configuration file:** `vitest.config.ts` or `jest.config.js` in `next-js-app/`

**Mocking library:** `vi.mock()` (Vitest) or `jest.mock()` for API responses

## Test File Organization

**Location pattern (when implemented):**
- Co-located with source: `feature.test.tsx` next to `feature.tsx`
- Or: `__tests__/feature.test.tsx` in same directory

**Naming convention:**
- `.test.ts` for utilities and hooks
- `.test.tsx` for React components
- `.spec.ts` / `.spec.tsx` alternative naming

**Structure example (not yet implemented):**
```
src/
├── entities/goal/
│   ├── ui/
│   │   ├── GoalTable.tsx
│   │   └── GoalTable.test.tsx
│   ├── api/
│   │   ├── goal-queries.ts
│   │   └── goal-queries.test.ts
│   └── index.ts
```

## Test Structure (Recommended Pattern)

**Test suite organization:**
```typescript
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';

describe('GoalTable', () => {
  let mockData;

  beforeEach(() => {
    mockData = [
      { id: '1', title: 'Goal 1', status: 'active' },
      { id: '2', title: 'Goal 2', status: 'completed' }
    ];
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  it('should render goal rows', () => {
    // test implementation
  });

  it('should handle row click', () => {
    // test implementation
  });
});
```

**Patterns to follow:**
- Use `describe()` blocks for logical grouping by component/function
- `beforeEach()` for common setup, `afterEach()` for cleanup
- One assertion per `it()` block (or related assertions testing one behavior)
- Descriptive test names: "should X when Y"

## Mocking

**What to mock:**
- API responses and openapi-fetch client
- React Query hooks
- Next.js navigation (`useRouter`, `useSearchParams`)
- External services (Clerk, etc.)

**What NOT to mock:**
- React Hook Form — use real instances in tests
- Zod schemas — validate actual schema logic
- Tailwind CSS classes
- Internal component composition

**API client mocking pattern (recommended):**
```typescript
import { vi } from 'vitest';
import { getApiClient } from '@/shared/api';

vi.mock('@/shared/api', () => ({
  getApiClient: vi.fn(() => ({
    GET: vi.fn().mockResolvedValue({
      data: { data: mockGoals },
      error: null
    })
  }))
}));
```

**React Query mocking pattern (recommended):**
```typescript
import { useQuery } from '@tanstack/react-query';
import { vi } from 'vitest';

vi.mock('@tanstack/react-query', async () => {
  const actual = await vi.importActual('@tanstack/react-query');
  return {
    ...actual,
    useQuery: vi.fn(() => ({
      data: mockData,
      isLoading: false,
      isError: false,
      error: null
    }))
  };
});
```

## Fixtures and Test Data

**Test data organization (when implemented):**
- Create `__fixtures__/` or `fixtures/` directories at feature/entity level
- Share factory functions for creating domain objects

**Example fixture (recommended pattern):**
```typescript
// entities/goal/__fixtures__/goal.fixtures.ts
export const createMockGoal = (overrides?: Partial<Goal>): Goal => ({
  id: '1',
  title: 'Test Goal',
  description: 'Test description',
  status: 'active',
  progress: 0,
  ...overrides
});

export const mockGoals = [
  createMockGoal({ id: '1', title: 'Goal 1' }),
  createMockGoal({ id: '2', title: 'Goal 2' })
];
```

## Coverage

**Current status:** Not measured

**Recommendations for implementation:**
- Target: 70%+ coverage for business logic layers
- Priority: entities, features, mutations, schemas
- Lower priority: UI rendering (component snapshots less valuable)

**View coverage when configured:**
```bash
pnpm test --coverage
# or
vitest --coverage
```

**Coverage report location:** `coverage/` directory (add to `.gitignore`)

## Test Types

### Unit Tests

**Scope:**
- Individual functions, hooks, utilities
- Zod schema validation
- Query key factories
- Mutation logic

**Approach:**
- Test pure functions with various inputs
- Verify error handling
- Mock external dependencies

**Example (utilities):**
```typescript
import { describe, it, expect } from 'vitest';
import { cn } from '@/shared/lib/utils';

describe('cn utility', () => {
  it('should merge class names', () => {
    expect(cn('px-2', 'px-4')).toBe('px-4'); // Tailwind merge
  });

  it('should handle conditional classes', () => {
    expect(cn('text-black', false && 'text-white')).toBe('text-black');
  });
});
```

### Integration Tests

**Scope:**
- Query hooks with mocked API client
- Mutations with cache invalidation
- Feature workflows (create → update → delete)
- Server actions with database interaction (when DB is testable)

**Approach:**
- Use `@testing-library/react` for component integration tests
- Mock API client, not individual HTTP calls
- Verify React Query cache updates

**Example (mutation with cache):**
```typescript
import { renderHook, act } from '@testing-library/react';
import { useCreateGoal } from '@/features/goals/create-goal';
import { getApiClient } from '@/shared/api';

vi.mock('@/shared/api');

describe('useCreateGoal', () => {
  it('should invalidate goal queries on success', async () => {
    const mockCreate = vi.fn().mockResolvedValue({
      data: { data: { id: '1', title: 'New Goal' } },
      error: null
    });

    vi.mocked(getApiClient).mockReturnValue({
      POST: mockCreate
    });

    const { result } = renderHook(() => useCreateGoal(), { wrapper: QueryClientWrapper });

    await act(async () => {
      await result.current.mutate({ title: 'New Goal' });
    });

    expect(mockCreate).toHaveBeenCalled();
    // Verify queryClient invalidation occurred
  });
});
```

### E2E Tests

**Status:** Not currently configured

**When to implement:**
- After backend stabilizes
- For critical user flows: login → create goal → check activity → log completion
- Use Playwright or Cypress

**Recommendation:** Defer until feature-complete; focus on integration tests first.

## Common Patterns

### Async Testing

**React Query hook testing (recommended):**
```typescript
import { renderHook, waitFor } from '@testing-library/react';
import { useGoals } from '@/entities/goal/api/goal-queries';

describe('useGoals', () => {
  it('should fetch goals', async () => {
    const { result } = renderHook(() => useGoals(), { wrapper: QueryClientWrapper });

    expect(result.current.isLoading).toBe(true);

    await waitFor(() => {
      expect(result.current.isLoading).toBe(false);
    });

    expect(result.current.data).toBeDefined();
  });
});
```

**Server action testing:**
```typescript
import { createActivityAction } from '@/features/activities/create-activity/api/create-activity-action';

describe('createActivityAction', () => {
  it('should throw on API error', async () => {
    const mockClient = {
      POST: vi.fn().mockResolvedValue({
        data: null,
        error: true,
        response: { status: 500 }
      })
    };

    vi.mocked(getServerApiClient).mockResolvedValue(mockClient);

    await expect(createActivityAction({ name: 'Test', code: 'TST' }))
      .rejects
      .toThrow('Failed to create activity');
  });
});
```

### Error Testing

**Pattern for error cases:**
```typescript
describe('ActivityService.CreateAsync', () => {
  it('should return validation error when name is empty', async () => {
    const service = new ActivityService(mockRepo, mockMapper);

    const result = await service.CreateAsync({
      name: '',
      code: 'TST'
    }, userId);

    expect(result.IsSuccess).toBe(false);
    expect(result.Error.ErrorType).toBe(OperationResultErrorType.Validation);
    expect(result.Error.ErrorMessage).toBe('Name is required');
  });

  it('should return not found when activity does not belong to user', async () => {
    const service = new ActivityService(mockRepo, mockMapper);
    const anotherUserId = Guid.NewGuid();

    const result = await service.UpdateAsync({
      id: existingActivityId,
      name: 'Updated',
      code: 'UPD'
    }, anotherUserId);

    expect(result.IsSuccess).toBe(false);
    expect(result.Error.ErrorType).toBe(OperationResultErrorType.NotFound);
  });
});
```

### Form Validation Testing

**Zod schema testing:**
```typescript
import { describe, it, expect } from 'vitest';
import { createActivitySchema } from '@/features/activities/create-activity/model/create-activity-schema';

describe('createActivitySchema', () => {
  it('should validate correct input', () => {
    const result = createActivitySchema.safeParse({
      name: 'Programming',
      code: 'PROG'
    });

    expect(result.success).toBe(true);
  });

  it('should fail when name is missing', () => {
    const result = createActivitySchema.safeParse({
      code: 'PROG'
    });

    expect(result.success).toBe(false);
    expect(result.error.flatten().fieldErrors.name).toBeDefined();
  });

  it('should fail when code exceeds 5 characters', () => {
    const result = createActivitySchema.safeParse({
      name: 'Programming',
      code: 'PROGRAMMING'
    });

    expect(result.success).toBe(false);
  });
});
```

## Backend Testing Patterns (C#)

### Service Testing (Recommended)

```csharp
public class ActivityServiceTests
{
    private readonly Mock<IActivityRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ActivityService _service;

    public ActivityServiceTests()
    {
        _mockRepository = new Mock<IActivityRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new ActivityService(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidInput_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateActivityRequest { Name = "Programming", Code = "PROG" };
        var activity = new Activity { Id = Guid.NewGuid(), Name = "Programming", Code = "PROG" };
        var userId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<Activity>()))
            .ReturnsAsync(activity);

        _mockMapper
            .Setup(m => m.Map<ActivityDto>(It.IsAny<Activity>()))
            .Returns(new ActivityDto { Name = "Programming", Code = "PROG" });

        // Act
        var result = await _service.CreateAsync(request, userId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Activity>()), Times.Once);
    }

    [Theory]
    [InlineData("", "PROG")]
    [InlineData("Name", "TOOLONG")]
    public async Task CreateAsync_WithInvalidInput_ReturnsValidationError(string name, string code)
    {
        // Arrange
        var request = new CreateActivityRequest { Name = name, Code = code };

        // Act
        var result = await _service.CreateAsync(request, Guid.NewGuid());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(OperationResultErrorType.Validation, result.Error.ErrorType);
    }
}
```

### OperationResult Testing

The `OperationResult<T>` pattern is tested implicitly through service tests. Verify:
- `IsSuccess` is true and `Data` is populated on success
- `IsSuccess` is false and `Error` is set on failure
- Implicit conversion operators work: `return OperationResultError.NotFound()`

## Test Helpers (When Implemented)

**Recommended test utilities:**

```typescript
// __tests__/setup.ts
import { QueryClient } from '@tanstack/react-query';
import { ReactNode } from 'react';

export function createTestQueryClient() {
  return new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false }
    }
  });
}

export function QueryClientWrapper({ children }: { children: ReactNode }) {
  const testQueryClient = createTestQueryClient();
  return (
    <QueryClientProvider client={testQueryClient}>
      {children}
    </QueryClientProvider>
  );
}
```

---

*Testing analysis: 2026-02-14*
