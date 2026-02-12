'use server';

import type { CreateGoalPayload } from '@/shared/api';
import { getServerApiClient } from '@/shared/api/server';

export async function createGoalAction(payload: CreateGoalPayload) {
  const client = await getServerApiClient();
  const { data, error, response } = await client.POST('/api/goals', { body: payload });

  if (error) {
    throw new Error(`Failed to create goal: ${response.status}`);
  }

  return data.data;
}
