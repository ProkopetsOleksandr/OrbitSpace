'use client';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useRouter } from 'next/navigation';
import { useState, useTransition } from 'react';
import { registerSchema, type RegisterInput } from '@/entities/auth';
import { Button } from '@/shared/ui/button';
import { Input } from '@/shared/ui/input';
import { Label } from '@/shared/ui/label';

export function RegisterForm() {
  const router = useRouter();
  const [error, setError] = useState<string | null>(null);
  const [isPending, startTransition] = useTransition();

  const {
    register,
    handleSubmit,
    formState: { errors }
  } = useForm<RegisterInput>({
    resolver: zodResolver(registerSchema)
  });

  const onSubmit = handleSubmit((data) => {
    startTransition(async () => {
      setError(null);

      // Step 1: Register the user
      const registerRes = await fetch('/api/auth/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          email: data.email,
          password: data.password,
          firstName: data.firstName,
          lastName: data.lastName,
          dateOfBirth: data.dateOfBirth.toISOString()
        })
      });

      if (!registerRes.ok) {
        const body = await registerRes.json().catch(() => ({ error: 'Registration failed' }));
        setError(body.error ?? 'Registration failed');
        return;
      }

      // Step 2: Auto-login with the same credentials
      const loginRes = await fetch('/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          email: data.email,
          password: data.password,
          rememberMe: false
        })
      });

      if (!loginRes.ok) {
        // Registration succeeded but login failed - redirect to login page
        router.push('/login?message=Registration successful. Please sign in.');
        return;
      }

      // Successfully registered and logged in
      router.push('/');
      router.refresh();
    });
  });

  return (
    <form onSubmit={onSubmit} className="space-y-4">
      <div className="grid grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="firstName">First name</Label>
          <Input
            {...register('firstName')}
            id="firstName"
            type="text"
            placeholder="John"
            disabled={isPending}
          />
          {errors.firstName && (
            <p className="text-sm text-red-500">{errors.firstName.message}</p>
          )}
        </div>

        <div className="space-y-2">
          <Label htmlFor="lastName">Last name</Label>
          <Input
            {...register('lastName')}
            id="lastName"
            type="text"
            placeholder="Doe"
            disabled={isPending}
          />
          {errors.lastName && <p className="text-sm text-red-500">{errors.lastName.message}</p>}
        </div>
      </div>

      <div className="space-y-2">
        <Label htmlFor="email">Email</Label>
        <Input
          {...register('email')}
          id="email"
          type="email"
          placeholder="you@example.com"
          disabled={isPending}
        />
        {errors.email && <p className="text-sm text-red-500">{errors.email.message}</p>}
      </div>

      <div className="space-y-2">
        <Label htmlFor="dateOfBirth">Date of birth</Label>
        <Input
          {...register('dateOfBirth')}
          id="dateOfBirth"
          type="date"
          disabled={isPending}
        />
        {errors.dateOfBirth && (
          <p className="text-sm text-red-500">{errors.dateOfBirth.message}</p>
        )}
      </div>

      <div className="space-y-2">
        <Label htmlFor="password">Password</Label>
        <Input
          {...register('password')}
          id="password"
          type="password"
          placeholder="Create a password"
          disabled={isPending}
        />
        {errors.password && <p className="text-sm text-red-500">{errors.password.message}</p>}
      </div>

      <div className="space-y-2">
        <Label htmlFor="repeatPassword">Confirm password</Label>
        <Input
          {...register('repeatPassword')}
          id="repeatPassword"
          type="password"
          placeholder="Repeat your password"
          disabled={isPending}
        />
        {errors.repeatPassword && (
          <p className="text-sm text-red-500">{errors.repeatPassword.message}</p>
        )}
      </div>

      {error && (
        <div className="rounded-md bg-red-50 p-3 text-sm text-red-800 border border-red-200">
          {error}
        </div>
      )}

      <Button type="submit" className="w-full" disabled={isPending}>
        {isPending ? 'Creating account...' : 'Create account'}
      </Button>
    </form>
  );
}
