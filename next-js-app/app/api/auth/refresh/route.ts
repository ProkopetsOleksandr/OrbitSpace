import { NextRequest, NextResponse } from 'next/server';
import { cookies } from 'next/headers';

export async function POST(request: NextRequest) {
  try {
    const cookieStore = await cookies();
    const refreshToken = cookieStore.get('refresh-token')?.value;

    if (!refreshToken) {
      return NextResponse.json({ error: 'No refresh token' }, { status: 401 });
    }

    // Forward to .NET backend
    const backendRes = await fetch(`${process.env.BACKEND_BASE_URL}/api/authentication/refresh`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken })
    });

    if (!backendRes.ok) {
      // Refresh failed - clear cookies, force re-login
      cookieStore.delete('access-token');
      cookieStore.delete('refresh-token');
      return NextResponse.json({ error: 'Session expired' }, { status: 401 });
    }

    const { accessToken: newAccess, refreshToken: newRefresh } = await backendRes.json();

    // Update cookies with new tokens
    cookieStore.set('access-token', newAccess, {
      httpOnly: true,
      secure: process.env.NODE_ENV === 'production',
      sameSite: 'lax',
      maxAge: 60 * 15, // 15 minutes
      path: '/'
    });

    cookieStore.set('refresh-token', newRefresh, {
      httpOnly: true,
      secure: process.env.NODE_ENV === 'production',
      sameSite: 'lax',
      maxAge: 60 * 60 * 24 * 30, // 30 days (simplified - backend controls real expiration)
      path: '/'
    });

    return NextResponse.json({ success: true });
  } catch (error) {
    console.error('Refresh error:', error);
    return NextResponse.json({ error: 'Internal server error' }, { status: 500 });
  }
}
