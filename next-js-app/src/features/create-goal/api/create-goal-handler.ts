import { NextResponse } from 'next/server';

import { CreateGoalPayload, getServerApiClient } from '@/shared/api';

export async function createGoalHandler(request: Request) {
  const client = await getServerApiClient();
  const payload = (await request.json()) as CreateGoalPayload;

  const { data, error, response } = await client.POST('/api/goals', { body: payload });

  if (error) {
    return NextResponse.json(error, { status: response.status });
  }

  return NextResponse.json(data);
}
