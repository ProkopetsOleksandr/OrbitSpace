import { ApiClient } from '@/lib/api/client';
import { components } from '@/types/api-types';

type TodoItem = components['schemas']['TodoItem'];
type CreateTodoItemPayload = components['schemas']['CreateTodoItemPayload'];
type UpdateTodoItemPayload = components['schemas']['UpdateTodoItemPayload'];

export const createTaskApi = (client: ApiClient) => ({
  getAll: async (): Promise<TodoItem[]> => {
    const { data, error } = await client.GET('/api/todo-items');
    if (error) throw error;
    return data?.data || [];
  },

  getById: async (id: string): Promise<TodoItem> => {
    const { data, error } = await client.GET('/api/todo-items/{id}', {
      params: { path: { id } },
    });
    if (error) throw error;
    if (!data?.data) throw new Error('Task not found');
    return data.data;
  },

  create: async (payload: CreateTodoItemPayload): Promise<TodoItem> => {
    const { data, error } = await client.POST('/api/todo-items', {
      body: payload,
    });
    if (error) throw error;
    if (!data?.data) throw new Error('Failed to create task');
    return data.data;
  },

  update: async (payload: UpdateTodoItemPayload): Promise<void> => {
    const { error } = await client.PUT('/api/todo-items/{id}', {
      params: { path: { id: payload.id } },
      body: payload,
    });
    if (error) throw error;
  },

  delete: async (id: string): Promise<void> => {
    const { error } = await client.DELETE('/api/todo-items/{id}', {
      params: { path: { id } },
    });
    if (error) throw error;
  },
});
