import { z } from 'zod';

// Login schema
export const loginSchema = z.object({
  email: z.string().email('Invalid email address'),
  password: z.string().min(4, 'Password must be at least 4 characters'),
  rememberMe: z.boolean().optional().default(false)
});

export type LoginInput = z.infer<typeof loginSchema>;

// Register schema
export const registerSchema = z
  .object({
    email: z.string().email('Invalid email address'),
    password: z.string().min(4, 'Password must be at least 4 characters'),
    repeatPassword: z.string(),
    firstName: z.string().min(1, 'First name is required').max(50, 'First name is too long'),
    lastName: z.string().min(1, 'Last name is required').max(50, 'Last name is too long'),
    dateOfBirth: z.coerce
      .date()
      .refine((date) => {
        const today = new Date();
        const minDate = new Date(
          today.getFullYear() - 13,
          today.getMonth(),
          today.getDate()
        );
        return date <= minDate;
      }, 'You must be at least 13 years old')
  })
  .refine((data) => data.password === data.repeatPassword, {
    message: 'Passwords do not match',
    path: ['repeatPassword']
  });

export type RegisterInput = z.infer<typeof registerSchema>;
