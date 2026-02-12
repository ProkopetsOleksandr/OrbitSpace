'use server';

import type { UpdateTodoItemPayload } from '@/shared/api';
import { getServerApiClient } from '@/shared/api/server';

export async function updateTodoItemAction(id: string, payload: UpdateTodoItemPayload) {
  const client = await getServerApiClient();
  const { error, response } = await client.PUT('/api/todo-items/{id}', {
    params: { path: { id } },
    body: payload,
  });

  if (error) {
    throw new Error(`Failed to update todo item: ${response.status}`);
  }
}
