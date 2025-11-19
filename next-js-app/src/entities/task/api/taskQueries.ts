import { useApiClient } from '@/lib/api/client';
import { components } from '@/types/api-types';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { createTaskApi } from './taskApi';

type CreateTodoItemPayload = components['schemas']['CreateTodoItemPayload'];
type UpdateTodoItemPayload = components['schemas']['UpdateTodoItemPayload'];

export const taskKeys = {
  all: ['tasks'] as const,
  lists: () => [...taskKeys.all, 'list'] as const,
  list: (filters: string) => [...taskKeys.lists(), { filters }] as const,
  details: () => [...taskKeys.all, 'detail'] as const,
  detail: (id: string) => [...taskKeys.details(), id] as const,
};

export const useTasks = () => {
  const client = useApiClient();
  const api = createTaskApi(client);

  return useQuery({
    queryKey: taskKeys.lists(),
    queryFn: () => api.getAll(),
  });
};

export const useTask = (id: string) => {
  const client = useApiClient();
  const api = createTaskApi(client);

  return useQuery({
    queryKey: taskKeys.detail(id),
    queryFn: () => api.getById(id),
    enabled: !!id,
  });
};

export const useCreateTask = () => {
  const queryClient = useQueryClient();
  const client = useApiClient();
  const api = createTaskApi(client);

  return useMutation({
    mutationFn: (payload: CreateTodoItemPayload) => api.create(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: taskKeys.lists() });
    },
  });
};

export const useUpdateTask = () => {
  const queryClient = useQueryClient();
  const client = useApiClient();
  const api = createTaskApi(client);

  return useMutation({
    mutationFn: (payload: UpdateTodoItemPayload) => api.update(payload),
    onSuccess: async (data, variables) => {
      await queryClient.invalidateQueries({ queryKey: taskKeys.lists() });
      await queryClient.invalidateQueries({ queryKey: taskKeys.detail(variables.id) });
    },
  });
};

export const useDeleteTask = () => {
  const queryClient = useQueryClient();
  const client = useApiClient();
  const api = createTaskApi(client);

  return useMutation({
    mutationFn: (id: string) => api.delete(id),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: taskKeys.lists() });
    },
  });
};
