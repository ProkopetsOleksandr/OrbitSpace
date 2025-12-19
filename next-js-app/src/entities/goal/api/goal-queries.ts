import { useQuery } from '@tanstack/react-query';

import { getApiClient } from '@/shared/api';
import { goalQueryKeys } from './goal-query-keys';

export function useGoals(filters?: string) {
  const apiClient = getApiClient();

  return useQuery({
    queryKey: goalQueryKeys.list(filters ?? ''),
    queryFn: async () => {
      const res = await apiClient.GET('/api/goals');

      if (res.error) {
        throw res.error;
      }

      return res.data.data;
    }
  });
}
