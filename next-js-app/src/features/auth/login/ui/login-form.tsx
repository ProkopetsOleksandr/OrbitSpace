'use client';

import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation } from '@tanstack/react-query';
import { useState } from 'react';
import { useForm, type Resolver } from 'react-hook-form';

import { AUTH_ERROR_CODE, loginSchema, type LoginInput } from '@/entities/auth';
import { Button } from '@/shared/ui/button';
import { Checkbox } from '@/shared/ui/checkbox';
import { Input } from '@/shared/ui/input';
import { Label } from '@/shared/ui/label';
import { useLogin } from '../model/login-mutation';

// TODO: another mutation file
function useResendVerificationEmail() {
  return useMutation({
    mutationFn: async (email: string) => {
      await fetch('/api/auth/resend-verification-email', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email })
      });
    }
  });
}

export function LoginForm() {
  const mutation = useLogin();
  const resend = useResendVerificationEmail();
  const [submittedEmail, setSubmittedEmail] = useState('');

  const {
    register,
    handleSubmit,
    formState: { errors }
  } = useForm<LoginInput>({
    resolver: zodResolver(loginSchema) as Resolver<LoginInput>,
    defaultValues: {
      email: '',
      password: '',
      rememberMe: false
    }
  });

  const onSubmit = handleSubmit(data => {
    setSubmittedEmail(data.email);
    mutation.mutate(data);
  });

  const isEmailNotVerified = mutation.error?.errorCode === AUTH_ERROR_CODE.Auth.EmailNotVerified;

  return (
    <form onSubmit={onSubmit} className="space-y-4">
      <div className="space-y-2">
        <Label htmlFor="email">Email</Label>
        <Input {...register('email')} id="email" type="email" placeholder="you@example.com" disabled={mutation.isPending} />
        {errors.email && <p className="text-sm text-red-500">{errors.email.message}</p>}
      </div>
      <div className="space-y-2">
        <Label htmlFor="password">Password</Label>
        <Input
          {...register('password')}
          id="password"
          type="password"
          placeholder="Enter your password"
          disabled={mutation.isPending}
        />
        {errors.password && <p className="text-sm text-red-500">{errors.password.message}</p>}
      </div>
      <div className="flex items-center space-x-2">
        <Checkbox {...register('rememberMe')} id="rememberMe" disabled={mutation.isPending} />
        <Label htmlFor="rememberMe" className="text-sm font-normal cursor-pointer">
          Remember me for 30 days
        </Label>
      </div>

      {/* TODO: Isn't it another feature for FSD? */}
      {mutation.error &&
        (isEmailNotVerified ? (
          <div className="rounded-md bg-amber-50 p-3 text-sm text-amber-800 border border-amber-200 space-y-2">
            <p>Your email is not verified. Please check your inbox.</p>
            {resend.isSuccess ? (
              <p className="font-medium">Verification email sent!</p>
            ) : (
              <Button
                type="button"
                variant="link"
                className="h-auto p-0 text-amber-800 underline"
                onClick={() => resend.mutate(submittedEmail)}
                disabled={resend.isPending}>
                {resend.isPending ? 'Sending...' : 'Resend verification email'}
              </Button>
            )}
          </div>
        ) : (
          <div className="rounded-md bg-red-50 p-3 text-sm text-red-800 border border-red-200">{mutation.error.error}</div>
        ))}
      <Button type="submit" className="w-full" disabled={mutation.isPending}>
        {mutation.isPending ? 'Signing in...' : 'Sign in'}
      </Button>
    </form>
  );
}
