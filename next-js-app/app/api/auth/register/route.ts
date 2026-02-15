import { NextRequest, NextResponse } from 'next/server';
import { registerSchema } from '@/entities/auth';

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();
    const parsed = registerSchema.safeParse(body);

    if (!parsed.success) {
      return NextResponse.json(
        {
          error: 'Invalid input',
          fieldErrors: parsed.error.flatten().fieldErrors
        },
        { status: 400 }
      );
    }

    const { email, password, firstName, lastName, dateOfBirth } = parsed.data;

    // Forward to .NET backend
    const backendRes = await fetch(
      `${process.env.BACKEND_BASE_URL}/api/authentication/register`,
      {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          email,
          password,
          firstName,
          lastName,
          dateOfBirth: dateOfBirth.toISOString()
        })
      }
    );

    if (!backendRes.ok) {
      const err = await backendRes.json().catch(() => ({ message: 'Registration failed' }));
      return NextResponse.json(
        { error: err.message ?? 'Registration failed' },
        { status: backendRes.status }
      );
    }

    return NextResponse.json({ success: true });
  } catch (error) {
    console.error('Register error:', error);
    return NextResponse.json({ error: 'Internal server error' }, { status: 500 });
  }
}
