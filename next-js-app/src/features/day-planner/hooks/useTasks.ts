// import { taskApi } from '@/entities/task/api/taskApi';
import { apiClient } from '@/lib/api/client';
import { useQuery } from '@tanstack/react-query';

export const useTasks = () => {
  return useQuery({
    queryKey: ['tasks'],
    queryFn: ({ signal }) => apiClient.GET('/api/todo-items', { signal }).then(r => r.data)
  });
};
