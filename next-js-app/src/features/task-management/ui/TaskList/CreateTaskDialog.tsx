'use client';

import { Button } from '@/components/ui/button';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger
} from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { zodResolver } from '@hookform/resolvers/zod';
import { Plus } from 'lucide-react';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { useCreateTodoItem } from '../../api/todoItemQueries';
import { todoItemCreateSchema, type todoItemCreateSchemaType } from '../../model/taskSchemas';

export function CreateTaskDialog() {
  const [open, setOpen] = useState(false);
  const createTodoItem = useCreateTodoItem();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors }
  } = useForm<todoItemCreateSchemaType>({
    resolver: zodResolver(todoItemCreateSchema),
    defaultValues: {
      title: ''
    }
  });

  const onSubmit = (values: todoItemCreateSchemaType) => {
    createTodoItem.mutate(values, {
      onSuccess: () => {
        setOpen(false);
        reset();
      }
    });
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button className="cursor-pointer">
          <Plus className="mr-2 h-4 w-4" />
          Add Task
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Add Task</DialogTitle>
          <DialogDescription>Create a new task for your dashboard.</DialogDescription>
        </DialogHeader>
        <form id="create-task-form" onSubmit={handleSubmit(onSubmit)} className="grid gap-4 py-4">
          <div className="grid gap-2">
            <label
              htmlFor="title"
              className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70">
              Title
            </label>
            <Input id="title" placeholder="Task title" {...register('title')} />
            {errors.title && <p className="text-sm text-red-500">{errors.title.message}</p>}
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
