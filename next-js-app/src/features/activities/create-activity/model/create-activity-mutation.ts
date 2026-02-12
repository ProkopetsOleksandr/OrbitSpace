import { useMutation, useQueryClient } from '@tanstack/react-query';

import { activityQueryKeys } from '@/entities/activity';
import type { CreateActivityPayload } from '@/shared/api';
import { createActivityAction } from '../api/create-activity-action';
import { CreateActivityFormValues } from './create-activity-schema';

export function useCreateActivity() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (values: CreateActivityFormValues) => {
      const payload: CreateActivityPayload = {
        name: values.name,
        code: values.code
      };

      return createActivityAction(payload);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: activityQueryKeys.lists() });
    }
  });
}
