import type { NextRequest } from 'next/server';
import { NextResponse } from 'next/server';

export default async function middleware(req: NextRequest) {
  const token = req.auth;
  const baseUrl = req.nextUrl.origin;

  const isLoginPage = req.nextUrl.pathname.startsWith('/login');

  if (!token && !isLoginPage) {
    return NextResponse.redirect(baseUrl + '/login');
  }

  // token expired
  if (token && Date.now() >= token.validity.refresh_until * 1000) {
    const response = NextResponse.redirect(baseUrl + 'login');
    response.cookies.set('next-auth.session-token', '', { maxAge: 0 });
    response.cookies.set('next-auth.csrf-token', '', { maxAge: 0 });
    return response;
  }

  // token valid
  return NextResponse.next();
}

export const config = {
  // Исключаем статические файлы, API-маршруты NextAuth и т.д.
  matcher: ['/((?!api|_next/static|_next/image|favicon.ico|assets|.*\\.png$).*)']
};
