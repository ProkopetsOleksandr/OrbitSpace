import { z } from 'zod';

export const RegisterSchema = z
  .object({
    email: z.email('Invalid email format'),
    firstName: z.string().min(1, 'First name is required').max(50, 'First name too long'),
    lastName: z.string().min(1, 'Last name is required').max(50, 'Last name too long'),
    password: z.string().min(6, 'Password must be at least 6 characters'),
    repeatPassword: z.string()
  })
  .refine(data => data.password === data.repeatPassword, {
    error: 'Passwords do not match',
    path: ['repeatPassword']
  });

export type RegisterFormData = z.infer<typeof RegisterSchema>;
