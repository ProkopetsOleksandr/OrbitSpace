'use server';

import type { CreateActivityPayload } from '@/shared/api';
import { getServerApiClient } from '@/shared/api/server';

export async function createActivityAction(payload: CreateActivityPayload) {
  const client = await getServerApiClient();
  const { data, error, response } = await client.POST('/api/activities', { body: payload });

  if (error) {
    throw new Error(`Failed to create activity: ${response.status}`);
  }

  return data.data;
}
