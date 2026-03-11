'use client';

import { useMutation } from '@tanstack/react-query';
import { useRouter } from 'next/navigation';

import { type RegisterInput } from '@/entities/auth';
import { useApiClient } from '@/shared/api';

export function useRegister() {
  const router = useRouter();
  const apiClient = useApiClient();

  return useMutation({
    mutationFn: async (data: RegisterInput) => {
      const { repeatPassword, ...body } = data;

      const { error } = await apiClient.POST('/api/authentication/register', { body });

      if (error) {
        throw new Error(error.detail ?? 'Registration failed');
      }
    },
    onSuccess: () => {
      router.push('/auth/register/success');
    }
  });
}
