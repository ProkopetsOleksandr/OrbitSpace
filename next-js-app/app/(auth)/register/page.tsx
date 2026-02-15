import Link from 'next/link';
import { RegisterForm } from '@/features/auth/register';

export default function RegisterPage() {
  return (
    <div className="space-y-6">
      <div className="space-y-2 text-center">
        <h2 className="text-2xl font-semibold tracking-tight">Create your account</h2>
        <p className="text-sm text-muted-foreground">Start your OrbitSpace journey</p>
      </div>

      <RegisterForm />

      <div className="text-center text-sm">
        <p className="text-muted-foreground">
          Already have an account?{' '}
          <Link href="/login" className="text-foreground hover:underline underline-offset-4">
            Sign in
          </Link>
        </p>
      </div>
    </div>
  );
}
