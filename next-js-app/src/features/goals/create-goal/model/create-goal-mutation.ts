import { useMutation, useQueryClient } from '@tanstack/react-query';

import { goalQueryKeys } from '@/entities/goal';
import { CreateGoalPayload, getApiClient } from '@/shared/api';
import { createGoalFormValues } from './create-goal-schema';

export function useCreateGoal() {
  const clientApi = getApiClient();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (values: createGoalFormValues) => {
      const payload: CreateGoalPayload = {
        title: values.title,
        lifeArea: values.lifeArea,
        isActive: values.isActive,
        isSmartGoal: values.isSmartGoal,
        description: values.description ?? null,
        metrics: values.metrics ?? null,
        achievabilityRationale: values.achievabilityRationale ?? null,
        motivation: values.motivation ?? null,
        dueAtUtc: values.dueDate ? values.dueDate.toISOString() : null
      };

      const { data, error } = await clientApi.POST('/api/goals', { body: payload });
      if (error) throw error;

      return data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: goalQueryKeys.lists() });
    }
  });
}
