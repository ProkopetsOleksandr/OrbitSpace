'use client';

import { useTodoItems } from '../../api/todoItemQueries';
import { TaskTable } from './TaskTable';

export const TaskList = () => {
  const { data: todoItems, isLoading, error } = useTodoItems();

  return (
    <div>
      <div className="flex items-center justify-between space-y-2 mb-4">
        <div>
          <h2 className="text-2xl font-bold tracking-tight">Tasks</h2>
          <p className="text-muted-foreground">Manage your tasks and view their status.</p>
        </div>
      </div>
      <TaskTable data={todoItems || []} isLoading={isLoading} error={error} />
    </div>
  );
};
