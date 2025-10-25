'use client';

import { RegisterSchema, type RegisterFormData } from '@/schemas/registerSchema';
import { zodResolver } from '@hookform/resolvers/zod';
import { Loader2, Lock, Mail, User } from 'lucide-react';
import { signIn } from 'next-auth/react';
import { useRouter } from 'next/navigation';
import { useState } from 'react';
import { useForm } from 'react-hook-form';

export default function RegisterForm() {
  const router = useRouter();
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string>('');

  const {
    register,
    handleSubmit,
    formState: { errors }
  } = useForm<RegisterFormData>({
    resolver: zodResolver(RegisterSchema),
    defaultValues: {
      email: '',
      firstName: '',
      lastName: '',
      password: '',
      repeatPassword: ''
    },
    mode: 'onSubmit'
  });

  async function onSubmit(data: RegisterFormData) {
    setLoading(true);
    setError('');

    try {
      // Отправка на твой .NET API или next-auth
      const response = await fetch('/api/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          email: data.email,
          firstName: data.firstName,
          lastName: data.lastName,
          password: data.password // Отправляем только password
        })
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Registration failed');
      }

      // Если регистрация успешна, пробуем сразу войти
      const result = await signIn('credentials', {
        redirect: false,
        email: data.email,
        password: data.password
      });

      if (result?.error) {
        setError(result.error);
      } else if (result?.ok) {
        router.push('/dashboard');
      }
    } catch (e: any) {
      setError(e.message || 'Application error');
      console.error(e);
    } finally {
      setLoading(false);
    }
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-4 min-w-[300px] md:min-w-[350px]">
      {/* Email */}
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

      {/* First Name */}
      <div className="relative flex flex-col">
        <label htmlFor="firstName" className="text-sm font-medium text-gray-700">
          First Name
        </label>
        <div className="relative mt-1">
          <User className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
          <input
            {...register('firstName')}
            id="firstName"
            placeholder="Enter your first name"
            disabled={loading}
            className={`w-full pl-10 pr-3 py-2 rounded-lg border ${
              errors.firstName ? 'border-red-500' : 'border-gray-300'
            } focus:outline-none focus:ring-2 focus:ring-blue-500 transition-all duration-200 disabled:opacity-50 bg-white/80`}
          />
        </div>
        {errors.firstName && <p className="mt-1 text-sm text-red-500">{errors.firstName.message}</p>}
      </div>

      {/* Last Name */}
      <div className="relative flex flex-col">
        <label htmlFor="lastName" className="text-sm font-medium text-gray-700">
          Last Name
        </label>
        <div className="relative mt-1">
          <User className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
          <input
            {...register('lastName')}
            id="lastName"
            placeholder="Enter your last name"
            disabled={loading}
            className={`w-full pl-10 pr-3 py-2 rounded-lg border ${
              errors.lastName ? 'border-red-500' : 'border-gray-300'
            } focus:outline-none focus:ring-2 focus:ring-blue-500 transition-all duration-200 disabled:opacity-50 bg-white/80`}
          />
        </div>
        {errors.lastName && <p className="mt-1 text-sm text-red-500">{errors.lastName.message}</p>}
      </div>

      {/* Password */}
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

      {/* Repeat Password */}
      <div className="relative flex flex-col">
        <label htmlFor="repeatPassword" className="text-sm font-medium text-gray-700">
          Confirm Password
        </label>
        <div className="relative mt-1">
          <Lock className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
          <input
            {...register('repeatPassword')}
            id="repeatPassword"
            type="password"
            placeholder="Confirm your password"
            disabled={loading}
            className={`w-full pl-10 pr-3 py-2 rounded-lg border ${
              errors.repeatPassword ? 'border-red-500' : 'border-gray-300'
            } focus:outline-none focus:ring-2 focus:ring-blue-500 transition-all duration-200 disabled:opacity-50 bg-white/80`}
          />
        </div>
        {errors.repeatPassword && <p className="mt-1 text-sm text-red-500">{errors.repeatPassword.message}</p>}
      </div>

      {error && <div className="text-center text-sm text-red-500 bg-red-50 p-2 rounded-lg">{error}</div>}

      <button
        type="submit"
        disabled={loading}
        className="flex items-center justify-center gap-2 bg-blue-600 text-white py-2 px-4 rounded-lg font-medium hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 transition-all duration-200 disabled:opacity-50 cursor-pointer">
        {loading && <Loader2 className="animate-spin h-5 w-5" />}
        {loading ? 'Registering...' : 'Register'}
      </button>
    </form>
  );
}
