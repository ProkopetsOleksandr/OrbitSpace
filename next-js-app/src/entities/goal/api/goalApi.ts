import { useApiClient } from '@/shared/lib/api/apiClient';
import { CreateGoalPayload, Goal, UpdateGoalPayload } from '../model/types';

export function useGoalApi() {
  const client = useApiClient();

  return {
    getAll: async (): Promise<Goal[]> => {
      const res = await client.GET('/api/goals');
      return res.data!.data;
    },

    getById: async (id: string): Promise<Goal> => {
      const res = await client.GET('/api/goals/{id}', {
        params: { path: { id: id } }
      });
      return res.data!.data;
    },

    create: async (payload: CreateGoalPayload): Promise<Goal> => {
      const res = await client.POST('/api/goals', {
        body: payload
      });
      return res.data!.data;
    },

    update: async (payload: UpdateGoalPayload): Promise<void> => {
      await client.PUT('/api/goals/{id}', {
        params: { path: { id: payload.id } },
        body: payload
      });
    },

    delete: async (id: string): Promise<void> => {
      await client.DELETE('/api/goals/{id}', {
        params: { path: { id: id } }
      });
    }
  };
}
