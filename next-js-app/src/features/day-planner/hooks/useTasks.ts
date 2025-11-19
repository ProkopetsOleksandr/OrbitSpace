// import { taskApi } from '@/entities/task/api/taskApi';
import {apiClient, useApiClient} from '@/lib/api/client';
import { useQuery } from '@tanstack/react-query';
import {createTaskApi} from "@/entities/task/api/taskApi";

export const useTasks = () => {
    const client = useApiClient();
    const api = createTaskApi(client);

    return useQuery({
        queryKey: ["tasks"],
        queryFn: () => api.getAll(),
    });
};


