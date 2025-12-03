import { useGoalApi } from '@/entities/goal/api/goalApi';
import { CreateGoalPayload } from '@/entities/goal/model/types';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

const goalQueryKeys = {
  all: ['goals'] as const,

  lists: () => [...goalQueryKeys.all, 'list'] as const,
  list: (filters: string) => [...goalQueryKeys.lists(), filters] as const,

  details: () => [...goalQueryKeys.all, 'detail'] as const,
  detail: (id: string) => [...goalQueryKeys.details(), id] as const
};

export function useGoals(filters?: string) {
  const goalApi = useGoalApi();

  return useQuery({
    queryKey: goalQueryKeys.list(filters ?? ''),
    queryFn: () => goalApi.getAll()
  });
}

export function useCreateGoal() {
  const goalApi = useGoalApi();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: CreateGoalPayload) => goalApi.create(payload),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: goalQueryKeys.lists() });
    }
  });
}
