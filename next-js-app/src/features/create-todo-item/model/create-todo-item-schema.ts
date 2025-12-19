import * as z from 'zod';

export const createTodoItemSchema = z.object({
  title: z.string().min(1, 'Title is required')
});

export type createTodoItemFormValues = z.infer<typeof createTodoItemSchema>;
