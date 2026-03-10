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
  await apiClient.POST('/api/authentication/resend-verification-email', {
    body: parsed.data.email
  });

  // Anti-enumeration: always return 200 regardless of backend result
  return NextResponse.json({ success: true });
}
