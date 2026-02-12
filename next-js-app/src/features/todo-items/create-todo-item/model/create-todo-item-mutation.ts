import { useMutation, useQueryClient } from '@tanstack/react-query';

import { todoItemQueryKeys } from '@/entities/todo-item';
import type { CreateTodoItemPayload } from '@/shared/api';
import { createTodoItemAction } from '../api/create-todo-item-action';
import { createTodoItemFormValues } from './create-todo-item-schema';

export function useCreateTodoItem() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (values: createTodoItemFormValues) => {
      const payload: CreateTodoItemPayload = {
        title: values.title
      };

      return createTodoItemAction(payload);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: todoItemQueryKeys.lists() });
    }
  });
}
