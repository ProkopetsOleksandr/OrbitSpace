'use client';

import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useRouter } from 'next/navigation';

import { authQueryKeys, type AuthError, type LoginInput } from '@/entities/auth';

export function useLogin() {
  const router = useRouter();
  const queryClient = useQueryClient();

  return useMutation<void, AuthError, LoginInput>({
    mutationFn: async (data: LoginInput) => {
      const res = await fetch('/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
      });

      if (!res.ok) {
        const body = await res.json().catch(() => ({ error: 'Login failed', errorCode: 'UNKNOWN' }));
        throw body as AuthError;
      }
    },
    onSuccess: () => {
      // TODO: what is all of that?
      const params = new URLSearchParams(window.location.search);
      const callbackUrl = params.get('callbackUrl') ?? '/';
      queryClient.invalidateQueries({ queryKey: authQueryKeys.session() });
      router.push(callbackUrl);
      router.refresh();
    }
  });
}
