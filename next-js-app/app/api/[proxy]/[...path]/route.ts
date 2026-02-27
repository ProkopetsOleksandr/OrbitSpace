import { NextRequest, NextResponse } from 'next/server';

import { backendBaseUrl } from '@/shared/config';
import { getAccessToken } from '@/shared/lib/cookies';

async function proxyToBackend(request: NextRequest) {
  const token = await getAccessToken();

  if (!token) {
    return NextResponse.json({ error: 'Unauthorized' }, { status: 401 });
  }

  const backendPath = request.nextUrl.pathname.replace(/^\/api\/proxy/, '');
  const search = request.nextUrl.search;
  const url = `${backendBaseUrl}${backendPath}${search}`;

  const headers = new Headers();
  headers.set('Authorization', `Bearer ${token}`);

  const contentType = request.headers.get('Content-Type');
  if (contentType) {
    headers.set('Content-Type', contentType);
  }

  const res = await fetch(url, {
    method: request.method,
    headers,
    body: ['GET', 'HEAD'].includes(request.method) ? undefined : await request.text()
  });

  return new NextResponse(res.body, {
    status: res.status,
    headers: {
      'Content-Type': res.headers.get('Content-Type') ?? 'application/json'
    }
  });
}

export const GET = proxyToBackend;
export const POST = proxyToBackend;
export const PUT = proxyToBackend;
export const DELETE = proxyToBackend;
export const PATCH = proxyToBackend;
