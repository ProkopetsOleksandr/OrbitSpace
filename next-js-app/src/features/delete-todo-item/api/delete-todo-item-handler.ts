import { NextResponse } from 'next/server';

import { getServerApiClient } from '@/shared/api';

export async function deleteTodoItemHandler(_request: Request, { params }: { params: Promise<{ id: string }> }) {
  const apiClient = await getServerApiClient();
  const { id } = await params;

  const { error, response } = await apiClient.DELETE('/api/todo-items/{id}', { params: { path: { id } } });

  return error ? NextResponse.json(error, { status: response.status }) : new NextResponse(null, { status: 204 });
}
