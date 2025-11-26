'use client';

import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { TodoItem } from '@/entities/todoItem/model/types';
import { zodResolver } from '@hookform/resolvers/zod';
import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { useUpdateTodoItem } from '../../api/todoItemQueries';
import { todoItemUpdateSchema, type todoItemUpdateSchemaType } from '../../model/taskSchemas';

interface EditTaskDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  todoItem: TodoItem;
}

export function EditTaskDialog({ open, onOpenChange, todoItem }: EditTaskDialogProps) {
  const updateTodoItem = useUpdateTodoItem();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors }
  } = useForm<todoItemUpdateSchemaType>({
    resolver: zodResolver(todoItemUpdateSchema),
    defaultValues: {
      title: todoItem.title,
      status: todoItem.status
    }
  });

  useEffect(() => {
    if (open) {
      reset({
        title: todoItem.title,
        status: todoItem.status
      });
    }
  }, [open, todoItem, reset]);

  const onSubmit = (values: todoItemUpdateSchemaType) => {
    updateTodoItem.mutate(
      {
        id: todoItem.id,
        title: values.title,
        status: values.status
      },
      {
        onSuccess: () => {
          onOpenChange(false);
        }
      }
    );
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Edit Task</DialogTitle>
          <DialogDescription>Make changes to your task here.</DialogDescription>
        </DialogHeader>
        <form id="edit-task-form" onSubmit={handleSubmit(onSubmit)} className="grid gap-4 py-4">
          <div className="grid gap-2">
            <label htmlFor="edit-title" className="text-sm font-medium leading-none">
              Title
            </label>
            <Input id="edit-title" {...register('title')} />
            {errors.title && <p className="text-sm text-red-500">{errors.title.message}</p>}
          </div>
          <div className="grid gap-2">
            <label htmlFor="edit-status" className="text-sm font-medium leading-none">
              Status
            </label>
            <select
              id="edit-status"
              className="flex h-10 w-full items-center justify-between rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 [&>span]:line-clamp-1"
              {...register('status')}>
              <option value="New">New</option>
              <option value="InProgress">In Progress</option>
              <option value="Complete">Complete</option>
            </select>
            {errors.status && <p className="text-sm text-red-500">{errors.status.message}</p>}
          </div>
        </form>
        <DialogFooter>
          <Button type="submit" form="edit-task-form" disabled={updateTodoItem.isPending}>
            {updateTodoItem.isPending ? 'Saving...' : 'Save changes'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
