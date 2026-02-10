import * as z from 'zod';

export const createActivitySchema = z.object({
  name: z.string().min(1, 'Name is required'),
  code: z.string().min(1, 'Code is required').max(5, 'Code must be at most 5 characters')
});

export type CreateActivityFormValues = z.infer<typeof createActivitySchema>;
