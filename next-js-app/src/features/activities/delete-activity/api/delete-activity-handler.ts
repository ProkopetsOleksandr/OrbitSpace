import { NextResponse } from 'next/server';

import { getServerApiClient } from '@/shared/api/server';

export async function deleteActivityHandler(_request: Request, { params }: { params: Promise<{ id: string }> }) {
  const apiClient = await getServerApiClient();
  const { id } = await params;

  const { error, response } = await apiClient.DELETE('/api/activities/{id}', { params: { path: { id } } });

  return error ? NextResponse.json(error, { status: response.status }) : new NextResponse(null, { status: 204 });
}
