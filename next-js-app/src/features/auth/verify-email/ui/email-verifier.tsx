'use client';

import { useMutation } from '@tanstack/react-query';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';

interface Props {
  token: string | undefined;
}

export function EmailVerifier({ token }: Props) {
  const router = useRouter();

  const mutation = useMutation({
    mutationFn: async (t: string) => {
      const res = await fetch('/api/auth/verify-email', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ token: t })
      });
      if (!res.ok) {
        const body = await res.json().catch(() => ({}));
        throw new Error(body.error ?? 'Verification failed');
      }
    },
    onSuccess: () => {
      setTimeout(() => router.push('/auth/login?verified=true'), 3000);
    }
  });

  useEffect(() => {
    if (token) mutation.mutate(token);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (!token) {
    return (
      <div className="space-y-4 text-center">
        <p className="text-muted-foreground text-sm">Invalid verification link.</p>
        <Link href="/auth/login" className="text-sm text-primary hover:underline">
          Go to login
        </Link>
      </div>
    );
  }

  if (mutation.isPending) {
    return <p className="text-center text-sm text-muted-foreground">Verifying your email...</p>;
  }

  if (mutation.isSuccess) {
    return (
      <div className="space-y-4 text-center">
        <h2 className="text-xl font-semibold">Email verified!</h2>
        <p className="text-muted-foreground text-sm">Redirecting to login...</p>
      </div>
    );
  }

  return (
    <div className="space-y-4 text-center">
      <h2 className="text-xl font-semibold">Verification link has expired</h2>
      <p className="text-muted-foreground text-sm">{mutation.error?.message}</p>
      <Link href="/auth/login" className="text-sm text-primary hover:underline">
        Go to login
      </Link>
    </div>
  );
}
