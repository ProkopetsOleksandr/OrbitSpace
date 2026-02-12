'use server';

import { getServerApiClient } from '@/shared/api/server';

export async function deleteTodoItemAction(id: string) {
  const client = await getServerApiClient();
  const { error, response } = await client.DELETE('/api/todo-items/{id}', {
    params: { path: { id } },
  });

  if (error) {
    throw new Error(`Failed to delete todo item: ${response.status}`);
  }
}
