import { Orbit } from 'lucide-react';
import Link from 'next/link';
import LoginForm from './loginForm';

export default function Page() {
  return (
    <div
      id="card"
      className="relative bg-white/80 backdrop-blur-lg p-6 sm:p-8 rounded-2xl shadow-xl max-w-md w-full border border-gray-100/50">
      <div className="flex flex-col items-center mb-6">
        <div className="flex items-center gap-3 mb-2">
          <Orbit className="size-8 text-blue-600" />
          <h1 className="text-2xl sm:text-3xl font-extrabold text-gray-900 tracking-tight">Orbit Space</h1>
        </div>
        <p className="text-gray-600 text-sm sm:text-base">Welcome back! Sign in to continue.</p>
      </div>

      <div id="card-body" className="space-y-4">
        <LoginForm />
      </div>

      <div className="mt-6 justify-center items-center flex gap-1 text-sm">
        <p className=" text-gray-500">Don’t have an account?</p>
        <Link href="/auth/register" className="text-blue-600 hover:underline font-medium">
          Sign Up
        </Link>
      </div>
    </div>
  );
}
