'use client';

import { Plus } from 'lucide-react';

import { TodoItemTable, useTodoItems } from '@/entities/todo-item';
import { CreateTodoItemDialog } from '@/features/todo-items/create-todo-item';
import { Button } from '@/shared/ui/button';
import { TodoItemTableActions } from './TodoItemTableActions';

export const TodoItemList = () => {
  const { data: todoItems, isLoading, error } = useTodoItems();

  return (
    <div>
      <div className="flex items-center justify-between space-y-2 mb-4">
        <div>
          <h2 className="text-2xl font-bold tracking-tight">Tasks</h2>
          <p className="text-muted-foreground">Manage your tasks and view their status.</p>
          <CreateTodoItemDialog>
            <Button className="cursor-pointer">
              <Plus className="mr-2 h-4 w-4" />
              Add Task
            </Button>
          </CreateTodoItemDialog>
        </div>
      </div>
      <TodoItemTable
        data={todoItems || []}
        isLoading={isLoading}
        error={error}
        renderActions={todoItem => <TodoItemTableActions todoItem={todoItem} />}
      />
    </div>
  );
};
