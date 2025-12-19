import { NextResponse } from 'next/server';

import { CreateTodoItemPayload, getServerApiClient } from '@/shared/api';

export async function createTodoItemHandler(request: Request) {
  const client = await getServerApiClient();
  const payload = (await request.json()) as CreateTodoItemPayload;

  const { data, error, response } = await client.POST('/api/todo-items', { body: payload });

  return error ? NextResponse.json(error, { status: response.status }) : NextResponse.json(data.data);
}
