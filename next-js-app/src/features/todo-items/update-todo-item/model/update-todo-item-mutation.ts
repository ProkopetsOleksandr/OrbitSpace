import { useMutation, useQueryClient } from '@tanstack/react-query';

import { todoItemQueryKeys } from '@/entities/todo-item';
import { getApiClient, UpdateTodoItemPayload } from '@/shared/api';
import { updateTodoItemFormValues } from './update-todo-item-schema';

export function useUpdateTodoItem() {
  const apiClient = getApiClient();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (values: updateTodoItemFormValues) => {
      const payload: UpdateTodoItemPayload = {
        id: values.id,
        title: values.title,
        status: values.status
      };

      const { error } = await apiClient.PUT('/api/todo-items/{id}', {
        params: { path: { id: payload.id } },
        body: payload
      });

      if (error) throw error;
    },
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: todoItemQueryKeys.lists() });
      await queryClient.invalidateQueries({ queryKey: todoItemQueryKeys.detail(variables.id) });
    }
  });
}
