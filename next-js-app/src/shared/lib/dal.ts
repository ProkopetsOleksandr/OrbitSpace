import { cache } from 'react';
import { cookies } from 'next/headers';
import { redirect } from 'next/navigation';

/**
 * Data Access Layer: getSession()
 *
 * Wrapped in React.cache() to memoize within a single render pass.
 * If multiple Server Components call getSession() during one request,
 * only one cookie read happens.
 *
 * This is defense-in-depth — middleware is the first layer,
 * but getSession() verifies auth at the data access point.
 */
export const getSession = cache(async () => {
  const cookieStore = await cookies();
  const token = cookieStore.get('access-token')?.value;

  if (!token) {
    redirect('/login');
  }

  return { token };
});

/**
 * Data Access Layer: fetchFromApi()
 *
 * Secure fetch helper for Server Components.
 * Automatically injects Authorization header from cookie.
 * Redirects to /login on 401 (expired/invalid token).
 */
export async function fetchFromApi<T>(
  endpoint: string,
  options?: RequestInit
): Promise<T> {
  const { token } = await getSession();

  const res = await fetch(`${process.env.BACKEND_BASE_URL}${endpoint}`, {
    ...options,
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
      ...options?.headers
    }
  });

  if (res.status === 401) {
    redirect('/login');
  }

  if (!res.ok) {
    throw new Error(`API error: ${res.status} ${res.statusText}`);
  }

  return res.json();
}
