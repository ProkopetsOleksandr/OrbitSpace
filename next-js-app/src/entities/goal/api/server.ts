import { getServerApiClient } from '@/shared/api/server';
import { NextResponse } from 'next/server';

export async function getGoalsHandler() {
  const client = await getServerApiClient();
  const { data, error, response } = await client.GET('/api/goals');

  return error ? NextResponse.json(error, { status: response.status }) : NextResponse.json(data);
}

export async function getGoalByIdHandler(_request: Request, { params }: { params: Promise<{ id: string }> }) {
  const client = await getServerApiClient();
  const { id } = await params;

  const { data, error, response } = await client.GET('/api/goals/{id}', {
    params: { path: { id } }
  });

  return error ? NextResponse.json(error, { status: response.status }) : NextResponse.json(data);
}
