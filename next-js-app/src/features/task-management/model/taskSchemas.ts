import { TodoItemStatus } from '@/entities/todoItem/model/types';
import * as z from 'zod';

export const todoItemCreateSchema = z.object({
  title: z.string().min(1, 'Title is required')
});
export type todoItemCreateSchemaType = z.infer<typeof todoItemCreateSchema>;

export const todoItemUpdateSchema = z.object({
  title: z.string().min(1, 'Title is required'),
  status: z.enum(Object.values(TodoItemStatus))
});
export type todoItemUpdateSchemaType = z.infer<typeof todoItemUpdateSchema>;
