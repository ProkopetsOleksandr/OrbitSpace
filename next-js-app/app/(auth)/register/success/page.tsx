import Link from 'next/link';

export default function RegisterSuccessPage() {
  return (
    <div className="space-y-4 text-center">
      <h2 className="text-xl font-semibold">Check your inbox</h2>
      <p className="text-muted-foreground text-sm">
        We&apos;ve sent a verification email to your inbox. Click the link to confirm your account.
      </p>
      <Link href="/auth/login" className="text-sm text-primary hover:underline">
        Back to sign in
      </Link>
    </div>
  );
}
