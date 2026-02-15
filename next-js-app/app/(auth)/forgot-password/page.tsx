import Link from 'next/link';
import { Button } from '@/shared/ui/button';

export default function ForgotPasswordPage() {
  return (
    <div className="space-y-6 text-center">
      <div className="space-y-2">
        <h2 className="text-2xl font-semibold tracking-tight">Password Reset</h2>
        <p className="text-sm text-muted-foreground">Coming soon</p>
      </div>

      <div className="space-y-4">
        <p className="text-sm text-muted-foreground">
          Password reset functionality will be available in a future update.
        </p>
        <p className="text-sm text-muted-foreground">
          For now, please contact support if you need help accessing your account.
        </p>
      </div>

      <Link href="/login">
        <Button variant="outline" className="w-full">
          Back to login
        </Button>
      </Link>
    </div>
  );
}
