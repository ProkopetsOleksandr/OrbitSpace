import { getServerApiClient } from '@/shared/api';
import { NextResponse } from 'next/server';

export async function getTodoItemsHandler() {
  const client = await getServerApiClient();
  const { data, error, response } = await client.GET('/api/todo-items');

  return error ? NextResponse.json(error, { status: response.status }) : NextResponse.json(data.data);
}
