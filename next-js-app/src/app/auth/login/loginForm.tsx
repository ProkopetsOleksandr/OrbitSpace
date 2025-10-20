'use client';

import { LoginSchema, type LoginFormData } from '@/lib/schemas/loginSchema';
import { zodResolver } from '@hookform/resolvers/zod';
import { Loader2, Lock, Mail } from 'lucide-react'; // Иконки для полей и спиннера
import { signIn } from 'next-auth/react';
import { useRouter } from 'next/navigation';
import { useState } from 'react';
import { useForm } from 'react-hook-form';

export default function LoginForm() {
  const router = useRouter();
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string>('');

  const {
    register,
    handleSubmit,
    formState: { errors }
  } = useForm<LoginFormData>({
    resolver: zodResolver(LoginSchema),
    defaultValues: {
      email: 'orbitspace-test1@gmail.com',
      password: '1234567'
    },
    mode: 'onSubmit'
  });

  async function onSubmit(data: LoginFormData) {
    setLoading(true);
    setError('');

    try {
      const result = await signIn('credentials', {
        redirect: false,
        email: data.email,
        password: data.password
      });

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
    <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-4 min-w-[300px] md:min-w-[350px]">
      {/* Поле Email */}
      <div className="relative flex flex-col">
        <label htmlFor="email" className="text-sm font-medium text-gray-700">
          Email
        </label>
        <div className="relative mt-1">
          <Mail className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
          <input
            {...register('email')}
            id="email"
            placeholder="Enter your email"
            disabled={loading}
            className={`w-full pl-10 pr-3 py-2 rounded-lg border ${
              errors.email ? 'border-red-500' : 'border-gray-300'
            } focus:outline-none focus:ring-2 focus:ring-blue-500 transition-all duration-200 disabled:opacity-50 bg-white/80`}
          />
        </div>
        {errors.email && <p className="mt-1 text-sm text-red-500">{errors.email.message}</p>}
      </div>

      {/* Поле Password */}
      <div className="relative flex flex-col">
        <label htmlFor="password" className="text-sm font-medium text-gray-700">
          Password
        </label>
        <div className="relative mt-1">
          <Lock className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
          <input
            {...register('password')}
            id="password"
            type="password"
            placeholder="Enter your password"
            disabled={loading}
            className={`w-full pl-10 pr-3 py-2 rounded-lg border ${
              errors.password ? 'border-red-500' : 'border-gray-300'
            } focus:outline-none focus:ring-2 focus:ring-blue-500 transition-all duration-200 disabled:opacity-50 bg-white/80`}
          />
        </div>
        {errors.password && <p className="mt-1 text-sm text-red-500">{errors.password.message}</p>}
      </div>

      {/* Серверная ошибка */}
      {error && <div className="text-center text-sm text-red-500 bg-red-50 p-2 rounded-lg">{error}</div>}

      {/* Кнопка */}
      <button
        type="submit"
        disabled={loading}
        className="flex items-center justify-center gap-2 bg-blue-600 text-white py-2 px-4 rounded-lg font-medium hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 transition-all duration-200 disabled:opacity-50">
        {loading && <Loader2 className="animate-spin h-5 w-5" />}
        {loading ? 'Logging in...' : 'Login'}
      </button>
    </form>
  );
}
