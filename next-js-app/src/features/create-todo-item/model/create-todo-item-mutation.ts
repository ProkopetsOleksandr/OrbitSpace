import { useMutation, useQueryClient } from '@tanstack/react-query';

import { todoItemQueryKeys } from '@/entities/todo-item';
import { CreateTodoItemPayload, getApiClient } from '@/shared/api';
import { createTodoItemFormValues } from './create-todo-item-schema';

export function useCreateTodoItem() {
  const clientApi = getApiClient();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (values: createTodoItemFormValues) => {
      const payload: CreateTodoItemPayload = {
        title: values.title
      };

      const { data, error } = await clientApi.POST('/api/todo-items', { body: payload });
      if (error) throw error;

      return data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: todoItemQueryKeys.lists() });
    }
  });
}
