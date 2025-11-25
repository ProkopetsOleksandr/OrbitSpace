import { useTodoItemApi } from '@/entities/todoItem/api/todoItemApi';
import { CreateTodoItemPayload, UpdateTodoItemPayload } from '@/entities/todoItem/model/types';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

const todoItemQueryKeys = {
  all: ['todoItems'] as const,
  lists: () => [...todoItemQueryKeys.all, 'list'] as const,
  details: () => [...todoItemQueryKeys.all, 'detail'] as const,
  detail: (id: string) => [...todoItemQueryKeys.details(), id] as const
};

export function useTodoItems() {
  const todoItemApi = useTodoItemApi();

  return useQuery({
    queryKey: todoItemQueryKeys.lists(),
    queryFn: () => todoItemApi.getAll()
  });
}

export function useTodoItem(id: string) {
  const todoItemApi = useTodoItemApi();

  return useQuery({
    queryKey: todoItemQueryKeys.detail(id),
    queryFn: () => todoItemApi.getById(id)
  });
}

export function useCreateTodoItem() {
  const todoItemApi = useTodoItemApi();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: CreateTodoItemPayload) => todoItemApi.create(payload),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: todoItemQueryKeys.lists() });
    }
  });
}

export function useUpdateTodoItem() {
  const todoItemApi = useTodoItemApi();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: UpdateTodoItemPayload) => todoItemApi.update(payload),
    onSuccess: async (data, variables) => {
      await queryClient.invalidateQueries({ queryKey: todoItemQueryKeys.lists() });
      await queryClient.invalidateQueries({ queryKey: todoItemQueryKeys.detail(variables.id) });
    }
  });
}

export function useDeleteTodoItem() {
  const todoItemApi = useTodoItemApi();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => todoItemApi.delete(id),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: todoItemQueryKeys.lists() });
    }
  });
}
