import { NextResponse } from 'next/server';

import { CreateGoalPayload } from '@/shared/api';
import { getServerApiClient } from '@/shared/api/server';

export async function createGoalHandler(request: Request) {
  const client = await getServerApiClient();
  const payload = (await request.json()) as CreateGoalPayload;

  const { data, error, response } = await client.POST('/api/goals', { body: payload });

  return error ? NextResponse.json(error, { status: response.status }) : NextResponse.json(data.data);
}
