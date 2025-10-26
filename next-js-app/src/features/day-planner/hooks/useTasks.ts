import { taskApi } from '@/entities/task/api/taskApi';
import { useQuery } from '@tanstack/react-query';

export const useTasks = () => {
  return useQuery({
    queryKey: ['tasks'],
    queryFn: taskApi.getAll
  });
};
