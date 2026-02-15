import { NextRequest, NextResponse } from 'next/server';
import { cookies } from 'next/headers';

export async function GET(request: NextRequest) {
  try {
    const cookieStore = await cookies();
    const accessToken = cookieStore.get('access-token')?.value;

    if (!accessToken) {
      return NextResponse.json({ user: null }, { status: 401 });
    }

    // Decode JWT payload (base64 decode middle part)
    // JWT structure: header.payload.signature
    const parts = accessToken.split('.');
    if (parts.length !== 3) {
      return NextResponse.json({ user: null }, { status: 401 });
    }

    const payload = JSON.parse(
      Buffer.from(parts[1], 'base64').toString('utf8')
    );

    // Extract user info from JWT claims
    // The backend sets: sub (userId), email, given_name, family_name, birthdate
    return NextResponse.json({
      user: {
        id: payload.sub || payload.userId,
        email: payload.email,
        firstName: payload.given_name || payload.firstName,
        lastName: payload.family_name || payload.lastName,
        dateOfBirth: payload.birthdate || payload.dateOfBirth,
        emailVerified: payload.email_verified || false
      }
    });
  } catch (error) {
    console.error('Session error:', error);
    return NextResponse.json({ user: null }, { status: 401 });
  }
}
