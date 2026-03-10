import { NextRequest, NextResponse } from 'next/server';
import { z } from 'zod';

import { registerSchema } from '@/entities/auth';
import { getServerApiClient } from '@/shared/api/api-server-client';

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

  const apiClient = await getServerApiClient();
  const { error, response } = await apiClient.POST('/api/authentication/register', {
    body: { email, password, firstName, lastName }
  });

  if (error) {
    return NextResponse.json({ error: error.title ?? error.detail ?? 'Registration failed' }, { status: response.status });
  }

  return NextResponse.json({ success: true });
}
