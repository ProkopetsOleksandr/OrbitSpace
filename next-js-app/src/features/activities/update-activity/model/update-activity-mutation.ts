import { useMutation, useQueryClient } from '@tanstack/react-query';

import { activityQueryKeys } from '@/entities/activity';
import type { UpdateActivityPayload } from '@/shared/api';
import { updateActivityAction } from '../api/update-activity-action';
import { UpdateActivityFormValues } from './update-activity-schema';

export function useUpdateActivity() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, values }: { id: string; values: UpdateActivityFormValues }) => {
      const payload: UpdateActivityPayload = {
        id,
        name: values.name,
        code: values.code
      };

      return updateActivityAction(id, payload);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: activityQueryKeys.lists() });
    }
  });
}
