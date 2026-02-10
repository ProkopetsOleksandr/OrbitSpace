import { useMutation, useQueryClient } from '@tanstack/react-query';

import { activityQueryKeys } from '@/entities/activity';
import { UpdateActivityPayload, getApiClient } from '@/shared/api';
import { UpdateActivityFormValues } from './update-activity-schema';

export function useUpdateActivity() {
  const clientApi = getApiClient();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, values }: { id: string; values: UpdateActivityFormValues }) => {
      const payload: UpdateActivityPayload = {
        id,
        name: values.name,
        code: values.code
      };

      const { error } = await clientApi.PUT('/api/activities/{id}', {
        params: { path: { id } },
        body: payload
      });
      if (error) throw error;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: activityQueryKeys.lists() });
    }
  });
}
