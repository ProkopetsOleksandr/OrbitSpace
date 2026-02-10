import { useMutation, useQueryClient } from '@tanstack/react-query';

import { activityQueryKeys } from '@/entities/activity';
import { getApiClient } from '@/shared/api';

export function useDeleteActivity() {
  const apiClient = getApiClient();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      const { error } = await apiClient.DELETE('/api/activities/{id}', {
        params: { path: { id } }
      });
      if (error) throw error;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: activityQueryKeys.lists() });
    }
  });
}
