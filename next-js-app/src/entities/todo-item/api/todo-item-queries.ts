import { useQuery } from '@tanstack/react-query';

import { getApiClient } from '@/shared/api';
import { todoItemQueryKeys } from './todo-item-query-keys';

export function useTodoItems() {
  const apiClient = getApiClient();

  return useQuery({
    queryKey: todoItemQueryKeys.list(),
    queryFn: async () => {
      const res = await apiClient.GET('/api/todo-items');

      if (res.error) {
        throw res.error;
      }

      return res.data.data;
    }
  });
}
