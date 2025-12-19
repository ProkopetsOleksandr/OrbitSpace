import { TodoItemStatus } from '@/shared/api';
import * as z from 'zod';

export const updateTodoItemSchema = z.object({
  id: z.string(),
  title: z.string().min(1, 'Title is required'),
  status: z.enum(Object.values(TodoItemStatus))
});

export type updateTodoItemFormValues = z.infer<typeof updateTodoItemSchema>;
