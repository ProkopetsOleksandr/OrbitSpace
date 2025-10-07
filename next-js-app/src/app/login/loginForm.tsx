'use client';

import { useActionState } from 'react';
import { useFormStatus } from 'react-dom';
import { login } from './actions';

export default function LoginForm() {
  const [state, loginAction] = useActionState(login, undefined);

  return (
    <form action={loginAction} className="flex max-w-[300px] flex-col gap-2">
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
