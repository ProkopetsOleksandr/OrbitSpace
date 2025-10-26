import { axiosClient } from '@/lib/api/axios.client';
import { TaskItem } from '../model/types';

export const taskApi = {
  getAll: async (): Promise<TaskItem[]> => {
    return (await axiosClient.get('/tasks')).data;
  }
};
