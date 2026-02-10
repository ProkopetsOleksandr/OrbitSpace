import { NextResponse } from 'next/server';

import { UpdateActivityPayload } from '@/shared/api';
import { getServerApiClient } from '@/shared/api/server';

export async function updateActivityHandler(request: Request, { params }: { params: Promise<{ id: string }> }) {
  const apiClient = await getServerApiClient();
  const { id } = await params;
  const payload = (await request.json()) as UpdateActivityPayload;

  const { error, response } = await apiClient.PUT('/api/activities/{id}', { params: { path: { id } }, body: payload });

  return error ? NextResponse.json(error, { status: response.status }) : new NextResponse(null, { status: 204 });
}
