import { useMutation, useQueryClient } from '@tanstack/react-query';

import { activityQueryKeys } from '@/entities/activity';
import { CreateActivityPayload, getApiClient } from '@/shared/api';
import { CreateActivityFormValues } from './create-activity-schema';

export function useCreateActivity() {
  const clientApi = getApiClient();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (values: CreateActivityFormValues) => {
      const payload: CreateActivityPayload = {
        name: values.name,
        code: values.code
      };

      const { data, error } = await clientApi.POST('/api/activities', { body: payload });
      if (error) throw error;

      return data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: activityQueryKeys.lists() });
    }
  });
}
