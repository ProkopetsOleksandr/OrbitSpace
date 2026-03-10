'use client';

import { type RegisterInput } from '@/entities/auth';
import { useMutation } from '@tanstack/react-query';
import { useRouter } from 'next/navigation';

export function useRegister() {
  const router = useRouter();

  return useMutation({
    mutationFn: async (data: RegisterInput) => {
      const res = await fetch('/api/auth/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
      });
      if (!res.ok) {
        const body = await res.json().catch(() => ({ error: 'Registration failed' }));
        throw new Error(body.error ?? 'Registration failed');
      }
    },
    onSuccess: () => {
      router.push('/auth/register/success');
    }
  });
}
