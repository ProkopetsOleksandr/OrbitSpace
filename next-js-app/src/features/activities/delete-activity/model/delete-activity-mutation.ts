import { useMutation, useQueryClient } from '@tanstack/react-query';

import { activityQueryKeys } from '@/entities/activity';
import { deleteActivityAction } from '../api/delete-activity-action';

export function useDeleteActivity() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      return deleteActivityAction(id);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: activityQueryKeys.lists() });
    }
  });
}
