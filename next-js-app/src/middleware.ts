import { cookies } from 'next/headers';
import { NextRequest, NextResponse } from 'next/server';
import { decryptSession } from './lib/session';

export default async function middleware(req: NextRequest) {
  const { isAuthenticated, tokenExpired, tokenVerificationError } = await useTokenVerification();
  const isLoginPage = req.nextUrl.pathname.startsWith('/login');

  if (isAuthenticated && isLoginPage) {
    return NextResponse.redirect(new URL('/', req.nextUrl));
  }

  if (tokenExpired || tokenVerificationError) {
    const response = isLoginPage ? NextResponse.next() : NextResponse.redirect(new URL('/login', req.nextUrl));
    response.cookies.delete('session');
    return response;
  }

  if (!isAuthenticated && !isLoginPage) {
    return NextResponse.redirect(new URL('/login', req.nextUrl));
  }

  return NextResponse.next();
}

async function useTokenVerification(): Promise<{
  isAuthenticated?: boolean;
  tokenExpired?: boolean;
  tokenVerificationError?: boolean;
}> {
  try {
    const cookie = (await cookies()).get('session')?.value;
    if (!cookie) {
      return {
        isAuthenticated: false
      };
    }

    const session = await decryptSession(cookie);
    if (!session) {
      return {
        tokenExpired: true
      };
    }

    const isExpired = !session.exp || session.exp <= Math.floor(Date.now() / 1000);

    return {
      tokenExpired: isExpired,
      isAuthenticated: !isExpired
    };
  } catch (error) {
    return {
      tokenVerificationError: true
    };
  }
}
