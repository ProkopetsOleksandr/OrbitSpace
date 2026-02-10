import { NextResponse } from 'next/server';

import { CreateActivityPayload } from '@/shared/api';
import { getServerApiClient } from '@/shared/api/server';

export async function createActivityHandler(request: Request) {
  const client = await getServerApiClient();
  const payload = (await request.json()) as CreateActivityPayload;

  const { data, error, response } = await client.POST('/api/activities', { body: payload });

  return error ? NextResponse.json(error, { status: response.status }) : NextResponse.json(data.data);
}
