'use client';

import { signIn } from 'next-auth/react';
import { useActionState } from 'react';
import { useFormStatus } from 'react-dom';
import { z } from 'zod';

const loginSchema = z.object({
  email: z.string().min(4, 'Email must be at least 4 characters'),
  password: z.string().min(6, 'Password must be at least 6 characters')
});

type FormDataType = z.infer<typeof loginSchema>;

type FormState =
  | {
      errors?: string[];
      properties?: {
        email?: { errors: string[] };
        password?: { errors: string[] } | null;
      };
    }
  | undefined;

export default function LoginForm() {
  const [state, setState, isPending] = useActionState(
    async (prevState: FormState, formData: FormDataType): Promise<FormState> => {
      const validationResult = loginSchema.safeParse(formData);
      if (!validationResult.success) {
        return z.treeifyError(validationResult.error);
      }

      const { email, password } = validationResult.data;

      const result = await signIn('credentials', { email, password, redirect: true });
      if (result?.error) {
        return {
          errors: ['Invalid email or password'],
          properties: {
            email: { errors: result.error }, // Показываем ошибку от NextAuth
            password: null
          }
        };
      }
    },
    undefined
  );

  return (
    <form action={setState} className="flex max-w-[300px] flex-col gap-2">
      <div className="flex flex-col gap-2">
        <input id="email" name="email" placeholder="Email" defaultValue="Alex" />
      </div>
      {state?.properties?.email && <p className="text-red-500">{state.properties.email.errors}</p>}

      <div className="flex flex-col gap-2">
        <input id="password" name="password" type="password" placeholder="Password" defaultValue="1234567" />
      </div>
      {state?.properties?.password && <p className="text-red-500">{state.properties.password.errors}</p>}
      <SubmitButton />
    </form>
  );
}

function SubmitButton() {
  const { pending } = useFormStatus();

  return (
    <button disabled={pending} type="submit">
      Login
    </button>
  );
}
