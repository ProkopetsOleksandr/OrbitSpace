import {useTodoItemApi} from "@/entities/todoItem/api/todoItemApi";
import {useQuery} from "@tanstack/react-query";
import {todoItemQueryKeys} from "@/entities/todoItem/model/queryKeys";

export function useTodoItems() {
    const todoItemApi = useTodoItemApi();

    return useQuery({
        queryKey: todoItemQueryKeys.lists(),
        queryFn: () => todoItemApi.getAll(),
    });
}

export function useTodoItem(id: string) {
    const todoItemApi = useTodoItemApi();

    return useQuery({
        queryKey: todoItemQueryKeys.detail(id),
        queryFn: () => todoItemApi.getById(id),
    });
}