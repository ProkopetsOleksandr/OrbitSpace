'use client';

import { zodResolver } from '@hookform/resolvers/zod';
import { useState } from 'react';
import { useForm } from 'react-hook-form';

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
import { Input } from '@/shared/ui/input';
import { useCreateTodoItem } from '../model/create-todo-item-mutation';
import { createTodoItemSchema, type createTodoItemFormValues } from '../model/create-todo-item-schema';

export function CreateTodoItemDialog({ children }: { children: React.ReactNode }) {
  const [open, setOpen] = useState(false);
  const createTodoItem = useCreateTodoItem();

  const form = useForm<createTodoItemFormValues>({
    resolver: zodResolver(createTodoItemSchema),
    defaultValues: {
      title: ''
    }
  });

  const { formState } = form;

  const onSubmit = (values: createTodoItemFormValues) => {
    createTodoItem.mutate(values, {
      onSuccess: () => {
        setOpen(false);
        form.reset();
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
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Add Task</DialogTitle>
          <DialogDescription>Create a new task for your dashboard.</DialogDescription>
        </DialogHeader>
        <form id="create-task-form" onSubmit={form.handleSubmit(onSubmit)} className="grid gap-4 py-4">
          <div className="grid gap-2">
            <label
              htmlFor="title"
              className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70">
              Title
            </label>
            <Input id="title" placeholder="Task title" {...form.register('title')} />
            {formState.errors.title && <p className="text-sm text-red-500">{formState.errors.title.message}</p>}
          </div>
        </form>
        <DialogFooter>
          <Button type="submit" form="create-task-form" disabled={createTodoItem.isPending}>
            {createTodoItem.isPending ? 'Saving...' : 'Save changes'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
