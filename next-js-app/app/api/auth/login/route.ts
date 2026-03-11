import { NextRequest, NextResponse } from 'next/server';

import { loginSchema } from '@/entities/auth';
import { getServerApiClient } from '@/shared/api/api-server-client';
import { setAuthCookies } from '@/shared/lib/cookies';

export async function POST(request: NextRequest) {
  const body = await request.json();
  const parsed = loginSchema.safeParse(body);

  if (!parsed.success) {
    return NextResponse.json({ error: 'Invalid input' }, { status: 400 });
  }

  const { email, password, rememberMe } = parsed.data;

  const apiClient = await getServerApiClient();
  const { data, error, response } = await apiClient.POST('/api/authentication/login', {
    body: {
      email,
      password,
      rememberMe,
      deviceInfo: request.headers.get('user-agent') ?? null
    }
  });

  if (error) {
    // TODO: Fix errorCode (add to OpenApi schema)
    const raw = error as typeof error & { errorCode?: string };
    return NextResponse.json(
      { error: raw.detail ?? 'Authentication failed', errorCode: raw.errorCode ?? 'UNKNOWN' },
      { status: response.status }
    );
  }

  await setAuthCookies(data.accessToken, data.refreshToken, rememberMe);

  return NextResponse.json({ success: true });
}
