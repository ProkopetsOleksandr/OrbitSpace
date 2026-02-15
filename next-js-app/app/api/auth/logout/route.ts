import { NextRequest, NextResponse } from 'next/server';
import { cookies } from 'next/headers';

export async function POST(request: NextRequest) {
  try {
    const cookieStore = await cookies();
    const refreshToken = cookieStore.get('refresh-token')?.value;

    // Best-effort revocation on backend
    if (refreshToken) {
      await fetch(`${process.env.BACKEND_BASE_URL}/api/authentication/revoke`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ refreshToken })
      }).catch(() => {
        // Ignore errors - we'll delete cookies anyway
      });
    }

    // Delete both cookies
    cookieStore.delete('access-token');
    cookieStore.delete('refresh-token');

    return NextResponse.json({ success: true });
  } catch (error) {
    console.error('Logout error:', error);
    return NextResponse.json({ error: 'Internal server error' }, { status: 500 });
  }
}
