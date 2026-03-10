import { NextRequest, NextResponse } from 'next/server';
import { z } from 'zod';

import { getServerApiClient } from '@/shared/api/api-server-client';

const schema = z.object({ email: z.email() });

export async function POST(request: NextRequest) {
  const body = await request.json();
  const parsed = schema.safeParse(body);

  if (!parsed.success) {
    return NextResponse.json({ error: 'Invalid input' }, { status: 400 });
  }

  const apiClient = await getServerApiClient();
  const { error, response } = await apiClient.POST('/api/authentication/resend-verification-email', {
    body: parsed.data.email
  });

  if (error) {
    return NextResponse.json({ error: error.detail ?? 'Failed to resend verification email' }, { status: response.status });
  }

  return NextResponse.json({ success: true });
}
