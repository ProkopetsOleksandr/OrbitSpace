import { getServerClient } from '@/shared/api/api-server-client';
import { NextResponse } from 'next/server';

export async function getGoalsHandler() {
  const client = await getServerClient();
  const { data, error } = await client.GET('/api/goals');
  return error ? NextResponse.json(error) : NextResponse.json(data.data);
}

export async function getGoalByIdHandler(_request: Request, { params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  const client = await getServerClient();

  const { data, error } = await client.GET('/api/goals/{id}', {
    params: { path: { id } }
  });

  return error ? NextResponse.json(error) : NextResponse.json(data.data);
}
