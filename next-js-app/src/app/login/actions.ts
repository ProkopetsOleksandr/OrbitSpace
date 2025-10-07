'use server';

import { redirect } from 'next/navigation';
import { z } from 'zod';
import { createSession, deleteSession } from '../../lib/session';

const loginSchema = z.object({
  email: z.string().trim().min(4, { message: 'Invalid email address' }),
  password: z.string().min(6, { message: 'Password must be at least 6 characters' }).trim()
});

export async function login(_prevState: any, formData: FormData) {
  const result = loginSchema.safeParse(Object.fromEntries(formData));
  if (!result.success) {
    return z.treeifyError(result.error);
  }

  const { email, password } = result.data;

  const res = await fetch(`${process.env.BACKEND_BASE_URL}/api/auth/login`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      username: email,
      password: password
    })
  });

  if (!res.ok) {
    return {
      errors: ['Invalid email or password'],
      properties: {
        email: {
          errors: ['Invalid email or password']
        },
        password: {
          errors: []
        }
      }
    };
  }

  const data: { token: string } = await res.json();
  await createSession(data.token);
  redirect('/');
}

export async function logout() {
  await deleteSession();
  redirect('/login');
}
