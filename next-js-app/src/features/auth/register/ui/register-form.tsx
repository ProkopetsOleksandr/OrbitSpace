'use client';

import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';

import { registerSchema, type RegisterInput } from '@/entities/auth';
import { Button } from '@/shared/ui/button';
import { Input } from '@/shared/ui/input';
import { Label } from '@/shared/ui/label';
import { useRegister } from '../model/register-mutation';

export function RegisterForm() {
  const mutation = useRegister();

  const {
    register,
    handleSubmit,
    formState: { errors }
  } = useForm<RegisterInput>({
    resolver: zodResolver(registerSchema)
  });

  const onSubmit = handleSubmit(data => {
    mutation.mutate(data);
  });

  return (
    <form onSubmit={onSubmit} className="space-y-4">
      <div className="grid grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="firstName">First name</Label>
          <Input {...register('firstName')} id="firstName" type="text" placeholder="John" disabled={mutation.isPending} />
          {errors.firstName && <p className="text-sm text-red-500">{errors.firstName.message}</p>}
        </div>

        <div className="space-y-2">
          <Label htmlFor="lastName">Last name</Label>
          <Input {...register('lastName')} id="lastName" type="text" placeholder="Doe" disabled={mutation.isPending} />
          {errors.lastName && <p className="text-sm text-red-500">{errors.lastName.message}</p>}
        </div>
      </div>

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
          placeholder="Create a password"
          disabled={mutation.isPending}
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
          disabled={mutation.isPending}
        />
        {errors.repeatPassword && <p className="text-sm text-red-500">{errors.repeatPassword.message}</p>}
      </div>

      {mutation.error && (
        <div className="rounded-md bg-red-50 p-3 text-sm text-red-800 border border-red-200">{mutation.error.message}</div>
      )}

      <Button type="submit" className="w-full" disabled={mutation.isPending}>
        {mutation.isPending ? 'Creating account...' : 'Create account'}
      </Button>
    </form>
  );
}
