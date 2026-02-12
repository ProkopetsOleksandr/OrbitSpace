import { useMutation, useQueryClient } from '@tanstack/react-query';

import { todoItemQueryKeys } from '@/entities/todo-item';
import { deleteTodoItemAction } from '../api/delete-todo-item-action';

export function useDeleteTodoItem() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      return deleteTodoItemAction(id);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: todoItemQueryKeys.lists() });
    }
  });
}
