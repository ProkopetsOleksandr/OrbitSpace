import { useMutation, useQueryClient } from '@tanstack/react-query';

import { goalQueryKeys } from '@/entities/goal';
import type { CreateGoalPayload } from '@/shared/api';
import { createGoalAction } from '../api/create-goal-action';
import { createGoalFormValues } from './create-goal-schema';

export function useCreateGoal() {
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

      return createGoalAction(payload);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: goalQueryKeys.lists() });
    }
  });
}
