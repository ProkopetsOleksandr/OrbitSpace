import { useMutation, useQueryClient } from '@tanstack/react-query';

import { todoItemQueryKeys } from '@/entities/todo-item';
import type { UpdateTodoItemPayload } from '@/shared/api';
import { updateTodoItemAction } from '../api/update-todo-item-action';
import { updateTodoItemFormValues } from './update-todo-item-schema';

export function useUpdateTodoItem() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (values: updateTodoItemFormValues) => {
      const payload: UpdateTodoItemPayload = {
        id: values.id,
        title: values.title,
        status: values.status
      };

      return updateTodoItemAction(payload.id, payload);
    },
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: todoItemQueryKeys.lists() });
      await queryClient.invalidateQueries({ queryKey: todoItemQueryKeys.detail(variables.id) });
    }
  });
}
