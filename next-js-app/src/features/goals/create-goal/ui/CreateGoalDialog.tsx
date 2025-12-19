'use client';

import { zodResolver } from '@hookform/resolvers/zod';
import { useState } from 'react';
import { DefaultValues, useForm } from 'react-hook-form';

import { Button } from '@/shared/ui/button';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger
} from '@/shared/ui/dialog';
import { useCreateGoal } from '../model/create-goal-mutation';
import { createGoalFormValues, createGoalSchema } from '../model/create-goal-schema';
import { CreateGoalForm } from './CreateGoalForm';

const defaultValues: DefaultValues<createGoalFormValues> = {
  title: '',
  imageUrl: '',
  isActive: false,
  isSmartGoal: true
};

export const CreateGoalDialog = ({ children }: { children: React.ReactNode }) => {
  const [open, setOpen] = useState(false);
  const { mutate: createGoal, isPending } = useCreateGoal();

  const form = useForm<createGoalFormValues>({
    resolver: zodResolver(createGoalSchema),
    defaultValues
  });

  const { formState } = form;

  const onSubmit = (values: createGoalFormValues) => {
    console.log(values);

    createGoal(values, {
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
      <DialogContent className="sm:max-w-4xl">
        <DialogHeader>
          <DialogTitle>Set a New Goal</DialogTitle>
          <DialogDescription>Clarity is power. Define exactly what success looks like for you.</DialogDescription>
        </DialogHeader>
        <CreateGoalForm id="create-goal-form" form={form} onSubmit={onSubmit} />
        <DialogFooter>
          <Button type="submit" form="create-goal-form" disabled={isPending}>
            {isPending ? 'Saving...' : 'Save'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};
