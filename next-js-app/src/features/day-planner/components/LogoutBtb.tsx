'use client';

import { SessionProvider, signOut, useSession } from 'next-auth/react';

export default function LogoutBtb() {
  return (
    <SessionProvider>
      <Btn />
    </SessionProvider>
  );
}

function Btn() {
  const x = useSession();

  console.log(JSON.stringify(x));

  return (
    <div>
      <button onClick={() => signOut()}>Logout</button>
    </div>
  );
}
