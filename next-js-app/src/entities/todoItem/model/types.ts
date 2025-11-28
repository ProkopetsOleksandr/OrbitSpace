import { components, TodoItemStatus } from '@/shared/types/api-types';

export type TodoItem = components['schemas']['TodoItem'];
export type CreateTodoItemPayload = components['schemas']['CreateTodoItemPayload'];
export type UpdateTodoItemPayload = components['schemas']['UpdateTodoItemPayload'];
export { TodoItemStatus };
