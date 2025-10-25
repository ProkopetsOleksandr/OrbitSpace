import { auth } from '@/lib/auth';
import type { NextRequest } from 'next/server';
import { NextResponse } from 'next/server';

export default async function middleware(req: NextRequest) {
  const session = await auth();
  const baseUrl = req.nextUrl.origin;

  const isAuthenticated = !!session;
  const isLoginPage = req.nextUrl.pathname.startsWith('/auth/login') || req.nextUrl.pathname.startsWith('/auth/register');

  if (!isAuthenticated && !isLoginPage) {
    return NextResponse.redirect(baseUrl + '/auth/login');
  }

  return NextResponse.next();
}

export const config = {
  // Исключаем статические файлы, API-маршруты NextAuth и т.д.
  matcher: ['/((?!api|_next/static|_next/image|favicon.ico|assets|.*\\.png$).*)']
};
