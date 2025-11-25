import { useApiClient } from '@/lib/api/apiClient';
import { type CreateTodoItemPayload, type TodoItem, type UpdateTodoItemPayload } from '../model/types';

export function useTodoItemApi() {
  const client = useApiClient();

  return {
    getAll: async (): Promise<TodoItem[]> => {
      const res = await client.GET('/api/todo-items');
      return res.data!.data;
    },

    getById: async (id: string): Promise<TodoItem> => {
      const res = await client.GET('/api/todo-items/{id}', {
        params: { path: { id: id } }
      });
      return res.data!.data;
    },

    create: async (payload: CreateTodoItemPayload): Promise<TodoItem> => {
      const res = await client.POST('/api/todo-items', {
        body: payload
      });
      return res.data!.data;
    },

    update: async (payload: UpdateTodoItemPayload): Promise<void> => {
      await client.PUT('/api/todo-items/{id}', {
        params: { path: { id: payload.id } },
        body: payload
      });
    },

    delete: async (id: string): Promise<void> => {
      await client.DELETE('/api/todo-items/{id}', {
        params: { path: { id: id } }
      });
    }
  };
}
