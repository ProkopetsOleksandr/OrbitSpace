import { NextRequest, NextResponse } from 'next/server';
import { cookies } from 'next/headers';
import { loginSchema } from '@/entities/auth';

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();
    const parsed = loginSchema.safeParse(body);

    if (!parsed.success) {
      return NextResponse.json({ error: 'Invalid input' }, { status: 400 });
    }

    const { email, password, rememberMe } = parsed.data;

    // Forward credentials to .NET backend
    const backendRes = await fetch(`${process.env.BACKEND_BASE_URL}/api/authentication/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        email,
        password,
        rememberMe,
        deviceInfo: request.headers.get('user-agent') ?? null
      })
    });

    if (!backendRes.ok) {
      const err = await backendRes.json().catch(() => ({ message: 'Authentication failed' }));
      return NextResponse.json(
        { error: err.message ?? 'Authentication failed' },
        { status: 401 }
      );
    }

    const { accessToken, refreshToken, user } = await backendRes.json();

    // Store tokens in httpOnly cookies
    const cookieStore = await cookies();

    cookieStore.set('access-token', accessToken, {
      httpOnly: true,
      secure: process.env.NODE_ENV === 'production',
      sameSite: 'lax',
      maxAge: 60 * 15, // 15 minutes
      path: '/'
    });

    cookieStore.set('refresh-token', refreshToken, {
      httpOnly: true,
      secure: process.env.NODE_ENV === 'production',
      sameSite: 'lax',
      maxAge: rememberMe ? 60 * 60 * 24 * 30 : 60 * 60 * 24 * 7, // 30 days or 7 days
      path: '/'
    });

    // Return only safe user data - never return raw tokens to the client
    return NextResponse.json({
      user: {
        id: user.id,
        email: user.email,
        firstName: user.firstName,
        lastName: user.lastName,
        dateOfBirth: user.dateOfBirth,
        emailVerified: user.emailVerified
      }
    });
  } catch (error) {
    console.error('Login error:', error);
    return NextResponse.json({ error: 'Internal server error' }, { status: 500 });
  }
}
