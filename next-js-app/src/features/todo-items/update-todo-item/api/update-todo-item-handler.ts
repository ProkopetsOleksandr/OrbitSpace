import { NextResponse } from 'next/server';

import { getServerApiClient, UpdateTodoItemPayload } from '@/shared/api';

export async function updateTodoItemHandler(request: Request, { params }: { params: Promise<{ id: string }> }) {
  const apiClient = await getServerApiClient();
  const { id } = await params;
  const payload = (await request.json()) as UpdateTodoItemPayload;

  const { error, response } = await apiClient.PUT('/api/todo-items/{id}', { params: { path: { id } }, body: payload });

  return error ? NextResponse.json(error, { status: response.status }) : new NextResponse(null, { status: 204 });
}
