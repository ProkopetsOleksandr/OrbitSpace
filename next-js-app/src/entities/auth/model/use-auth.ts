'use client';

import { useQuery } from '@tanstack/react-query';
import { authQueryKeys } from '../api/auth-query-keys';
import type { AuthUser } from './types';

export function useAuth() {
  const { data, isLoading } = useQuery({
    queryKey: authQueryKeys.session(),
    queryFn: async (): Promise<AuthUser | null> => {
      const res = await fetch('/api/auth/session');
      if (!res.ok) {
        return null;
      }
      const json = await res.json();
      return json.user ?? null;
    },
    staleTime: 5 * 60 * 1000, // 5 minutes
    retry: false
  });

  return {
    user: data ?? null,
    isAuthenticated: !!data,
    isLoading
  };
}
