import { useSession } from 'next-auth/react';

export default function MySession() {
  const session = useSession();

  return <div>{JSON.stringify(session)}</div>;
}
