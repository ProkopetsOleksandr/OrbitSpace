import { goalCreateSchema, goalCreateSchemaType } from '@/entities/goal/model/schemas';
import { CreateGoalPayload } from '@/entities/goal/model/types';
import { Button } from '@/shared/components/ui/button';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger
} from '@/shared/components/ui/dialog';
import { zodResolver } from '@hookform/resolvers/zod';
import { useState } from 'react';
import { DefaultValues, useForm } from 'react-hook-form';
import { useCreateGoal } from '../../api/goal-queries';
import { CreateGoalForm } from '../forms/CreateGoalForm';

const defaultValues: DefaultValues<goalCreateSchemaType> = {
  title: '',
  isActive: false,
  isSmartGoal: true
};

export const CreateGoalDialog = ({ children }: { children: React.ReactNode }) => {
  const [open, setOpen] = useState(false);
  const { mutate: createGoal, isPending } = useCreateGoal();

  const form = useForm<goalCreateSchemaType>({
    resolver: zodResolver(goalCreateSchema),
    defaultValues
  });

  const { formState } = form;

  const onSubmit = (values: goalCreateSchemaType) => {
    const payload: CreateGoalPayload = {
      title: values.title,
      lifeArea: values.lifeArea,
      isActive: values.isActive,
      isSmartGoal: values.isSmartGoal,
      description: values.description ?? null,
      metrics: values.metrics ?? null,
      achievabilityRationale: values.achievabilityRationale ?? null,
      motivation: values.motivation ?? null,
      dueDate: values.dueDate ? values.dueDate.toISOString() : null
    };

    console.log(payload);
    return;

    createGoal(payload, {
      onSuccess: () => {
        form.reset();
        setOpen(false);
      }
    });
  };

  const handleOpenChange = (newOpenState: boolean) => {
    if (newOpenState) {
      setOpen(true);
      form.reset();
      return;
    }

    if (formState.isDirty) {
      const confirmClose = window.confirm('You have unsaved changes. Are you sure you want to close?');
      if (!confirmClose) {
        return;
      }
    }

    setOpen(false);
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogTrigger asChild>{children}</DialogTrigger>
      <DialogContent className="max-h-[85vh] flex flex-col p-0 gap-0 sm:max-w-lg">
        <DialogHeader className="px-6 py-4 border-b">
          <DialogTitle>Set a New Goal</DialogTitle>
          <DialogDescription>Clarity is power. Define exactly what success looks like for you.</DialogDescription>
        </DialogHeader>
        <div className="flex-1 overflow-y-auto px-6 py-4">
          <CreateGoalForm id="create-goal-form" form={form} onSubmit={onSubmit} />
        </div>
        <DialogFooter className="px-6 py-4 border-t">
          <Button type="submit" form="create-goal-form" disabled={isPending}>
            {isPending ? 'Saving...' : 'Save'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};
