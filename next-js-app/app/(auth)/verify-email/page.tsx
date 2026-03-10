import { EmailVerifier } from '@/features/auth/verify-email';

interface Props {
  searchParams: Promise<{ token?: string }>;
}

export default async function VerifyEmailPage({ searchParams }: Props) {
  const { token } = await searchParams;
  return <EmailVerifier token={token} />;
}
