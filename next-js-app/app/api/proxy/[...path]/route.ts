import { auth } from '@clerk/nextjs/server';
import { NextRequest, NextResponse } from 'next/server';

import { backendBaseUrl } from '@/shared/config';

async function proxyToBackend(request: NextRequest, { params }: { params: Promise<{ path: string[] }> }) {
  const { getToken } = await auth();
  const token = await getToken();

  if (!token) {
    return NextResponse.json({ error: 'Unauthorized' }, { status: 401 });
  }

  const { path } = await params;
  const backendPath = path.join('/');
  const search = request.nextUrl.search;
  const url = `${backendBaseUrl}/${backendPath}${search}`;

  const headers = new Headers();
  headers.set('Authorization', `Bearer ${token}`);
  headers.set('Content-Type', request.headers.get('Content-Type') ?? 'application/json');

  const res = await fetch(url, {
    method: request.method,
    headers,
    body: ['GET', 'HEAD'].includes(request.method) ? undefined : await request.text(),
  });

  return new NextResponse(res.body, {
    status: res.status,
    headers: res.headers,
  });
}

export const GET = proxyToBackend;
export const POST = proxyToBackend;
export const PUT = proxyToBackend;
export const DELETE = proxyToBackend;
export const PATCH = proxyToBackend;
