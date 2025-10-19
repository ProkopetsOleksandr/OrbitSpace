'use client';

import { zodResolver } from '@hookform/resolvers/zod';
import { signIn } from 'next-auth/react';
import { useRouter } from 'next/navigation';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';

const LoginSchema = z.object({
  email: z.string().min(4, 'Email must be at least 4 characters'),
  password: z.string().min(6, 'Password must be at least 6 characters')
});

type LoginData = z.infer<typeof LoginSchema>;

export default function LoginForm2() {
  const router = useRouter();
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string>('');

  const { register, handleSubmit, formState } = useForm<LoginData>({
    resolver: zodResolver(LoginSchema),
    defaultValues: {
      email: 'Alex',
      password: '1234567'
    },
    mode: 'onBlur'
  });

  async function onSubmit(data: LoginData) {
    setLoading(true);

    try {
      const result = await signIn('credentials', {
        redirect: false,
        email: data.email,
        password: data.password
      });

      console.log(result);

      if (result?.error) {
        setError(result.error);
      } else if (result.ok) {
        router.push('/');
      }
    } catch (e) {
      setError('Application error');
      console.error(e);
    } finally {
      setLoading(false);
    }
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="flex min-w-[300px] flex-col gap-2">
      <div className="flex flex-col gap-2">
        <input
          {...register('email')}
          placeholder="Email"
          disabled={loading}
          className={`mt-1 block w-full rounded-md border ${
            formState.errors.email ? 'border-red-500' : 'border-gray-300'
          } shadow-sm p-2`}
        />
      </div>
      {formState.errors.email && <p className="text-red-500">{formState.errors.email.message}</p>}

      <div className="flex flex-col gap-2">
        <input
          {...register('password')}
          type="password"
          disabled={loading}
          placeholder="Password"
          className={`mt-1 block w-full rounded-md border ${
            formState.errors.password ? 'border-red-500' : 'border-gray-300'
          } shadow-sm p-2`}
        />
      </div>
      {formState.errors.password && <p className="text-red-500">{formState.errors.password.message}</p>}

      {error && <div>{error}</div>}

      <button disabled={loading} type="submit" className="bg-gray-200 p-2 rounded cursor-pointer">
        {loading ? 'Logging in...' : 'Login'}
      </button>
    </form>
  );
}
