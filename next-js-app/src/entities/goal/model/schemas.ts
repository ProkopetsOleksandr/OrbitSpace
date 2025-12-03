import { LifeArea } from '@/entities/goal/model/types';
import * as z from 'zod';

export const goalCreateSchema = z
  .object({
    // Basic Info
    title: z.string().min(1, 'Field is required'),
    lifeArea: z.enum(LifeArea, { error: 'Field is required' }),
    isActive: z.boolean(),
    isSmartGoal: z.boolean(),

    // SMART Goal Details
    description: z.string().optional(),
    metrics: z.string().optional(),
    achievabilityRationale: z.string().optional(),
    motivation: z.string().optional(),
    dueDate: z.date().optional()
  })
  .superRefine((data, ctx) => {
    if (!data.isSmartGoal) {
      return;
    }

    (['description', 'metrics', 'achievabilityRationale', 'motivation'] as const).forEach(field => {
      const value = data[field] as string;
      if (!value?.trim()) {
        ctx.addIssue({
          code: 'custom',
          message: 'Field is required for SMART goals',
          path: [field]
        });
      }
    });

    if (!data.dueDate) {
      ctx.addIssue({
        code: 'custom',
        message: 'Field is required for SMART goals',
        path: ['dueDate']
      });
    }
  });

export type goalCreateSchemaType = z.infer<typeof goalCreateSchema>;
