'use client';

/**
 * Token Refresh Queue
 *
 * Prevents race conditions when multiple requests fail with 401 simultaneously.
 * Only one refresh occurs — subsequent requests queue and wait for the result.
 */

let isRefreshing = false;
let refreshQueue: Array<{
  resolve: (value: void) => void;
  reject: (reason?: unknown) => void;
}> = [];

function processQueue(error: Error | null) {
  refreshQueue.forEach(({ resolve, reject }) => {
    if (error) {
      reject(error);
    } else {
      resolve(undefined);
    }
  });
  refreshQueue = [];
}

export async function handleTokenRefresh(): Promise<void> {
  // If already refreshing, queue this request
  if (isRefreshing) {
    return new Promise((resolve, reject) => {
      refreshQueue.push({ resolve, reject });
    });
  }

  isRefreshing = true;

  try {
    const res = await fetch('/api/auth/refresh', { method: 'POST' });

    if (!res.ok) {
      throw new Error('Refresh failed');
    }

    // Refresh succeeded — resolve all queued promises
    processQueue(null);
  } catch (error) {
    // Refresh failed — reject all queued promises and redirect to login
    processQueue(error as Error);
    window.location.href = '/login';
    throw error;
  } finally {
    isRefreshing = false;
  }
}
