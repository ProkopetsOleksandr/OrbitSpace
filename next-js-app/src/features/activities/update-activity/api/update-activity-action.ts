'use server';

import type { UpdateActivityPayload } from '@/shared/api';
import { getServerApiClient } from '@/shared/api/server';

export async function updateActivityAction(id: string, payload: UpdateActivityPayload) {
  const client = await getServerApiClient();
  const { error, response } = await client.PUT('/api/activities/{id}', {
    params: { path: { id } },
    body: payload,
  });

  if (error) {
    throw new Error(`Failed to update activity: ${response.status}`);
  }
}
