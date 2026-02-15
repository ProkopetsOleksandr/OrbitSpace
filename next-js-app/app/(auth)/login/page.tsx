import Link from 'next/link';
import { LoginForm } from '@/features/auth/login';

export default function LoginPage() {
  return (
    <div className="space-y-6">
      <div className="space-y-2 text-center">
        <h2 className="text-2xl font-semibold tracking-tight">Welcome back</h2>
        <p className="text-sm text-muted-foreground">Sign in to your account</p>
      </div>

      <LoginForm />

      <div className="space-y-2 text-center text-sm">
        <Link
          href="/forgot-password"
          className="text-muted-foreground hover:text-foreground underline underline-offset-4"
        >
          Forgot your password?
        </Link>
        <p className="text-muted-foreground">
          Don&apos;t have an account?{' '}
          <Link href="/register" className="text-foreground hover:underline underline-offset-4">
            Sign up
          </Link>
        </p>
      </div>
    </div>
  );
}
