'use server';

import type { CreateTodoItemPayload } from '@/shared/api';
import { getServerApiClient } from '@/shared/api/server';

export async function createTodoItemAction(payload: CreateTodoItemPayload) {
  const client = await getServerApiClient();
  const { data, error, response } = await client.POST('/api/todo-items', { body: payload });

  if (error) {
    throw new Error(`Failed to create todo item: ${response.status}`);
  }

  return data;
}
