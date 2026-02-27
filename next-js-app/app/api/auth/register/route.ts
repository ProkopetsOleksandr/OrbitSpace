import { NextRequest, NextResponse } from 'next/server';
import { z } from 'zod';

import { registerSchema } from '@/entities/auth';
import { getServerApiClient } from '@/shared/api/api-server-client';
import { backendBaseUrl } from '@/shared/config';

export async function POST(request: NextRequest) {
  const body = await request.json();
  const parsed = registerSchema.safeParse(body);

  if (!parsed.success) {
    return NextResponse.json(
      {
        error: 'Invalid input',
        fieldErrors: z.flattenError(parsed.error).fieldErrors
      },
      { status: 400 }
    );
  }

  const { email, password, firstName, lastName } = parsed.data;

  const serverApiClient = await getServerApiClient();

  const backendRes = await fetch(`${backendBaseUrl}/api/authentication/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password, firstName, lastName })
  });

  if (!backendRes.ok) {
    const err = await backendRes.json().catch(() => ({ message: 'Registration failed' }));
    return NextResponse.json({ error: err.message ?? 'Registration failed' }, { status: backendRes.status });
  }

  return NextResponse.json({ success: true });
}
