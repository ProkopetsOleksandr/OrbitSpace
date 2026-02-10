import { useQuery } from '@tanstack/react-query';

import { getApiClient } from '@/shared/api';
import { activityQueryKeys } from './activity-query-keys';

export function useActivities() {
  const apiClient = getApiClient();

  return useQuery({
    queryKey: activityQueryKeys.list(''),
    queryFn: async () => {
      const res = await apiClient.GET('/api/activities');

      if (res.error) {
        throw res.error;
      }

      return res.data.data;
    }
  });
}
