import { todoItemQueryKeys } from '@/entities/todo-item';
import { getApiClient } from '@/shared/api';
import { useMutation, useQueryClient } from '@tanstack/react-query';

export function useDeleteTodoItem() {
  const apiClient = getApiClient();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      const { error } = await apiClient.DELETE('/api/todo-items/{id}', {
        params: { path: { id } }
      });
      if (error) throw error;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: todoItemQueryKeys.lists() });
    }
  });
}
