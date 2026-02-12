'use server';

import { getServerApiClient } from '@/shared/api/server';

export async function deleteActivityAction(id: string) {
  const client = await getServerApiClient();
  const { error, response } = await client.DELETE('/api/activities/{id}', {
    params: { path: { id } },
  });

  if (error) {
    throw new Error(`Failed to delete activity: ${response.status}`);
  }
}
