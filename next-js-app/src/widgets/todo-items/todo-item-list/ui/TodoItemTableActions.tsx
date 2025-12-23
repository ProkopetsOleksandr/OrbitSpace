import { MoreHorizontal, Pencil, Trash } from 'lucide-react';
import { useState } from 'react';

import { DeleteTodoItemDialog } from '@/features/todo-items/delete-todo-item';
import { EditTodoItemDialog } from '@/features/todo-items/update-todo-item';
import { TodoItem } from '@/shared/api';
import { Button } from '@/shared/ui/button';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from '@/shared/ui/dropdown-menu';

interface TaskTableActionsProps {
  todoItem: TodoItem;
}

export function TodoItemTableActions({ todoItem }: TaskTableActionsProps) {
  const [showEditDialog, setShowEditDialog] = useState(false);
  const [showDeleteDialog, setShowDeleteDialog] = useState(false);

  return (
    <>
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button variant="ghost" className="h-8 w-8 p-0 cursor-pointer">
            <span className="sr-only">Open menu</span>
            <MoreHorizontal className="h-4 w-4" />
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align="end">
          <DropdownMenuItem onClick={() => setShowEditDialog(true)} className="cursor-pointer">
            <Pencil className="mr-2 h-4 w-4" />
            Edit
          </DropdownMenuItem>
          <DropdownMenuItem onClick={() => setShowDeleteDialog(true)} className="text-red-600 cursor-pointer">
            <Trash className="mr-2 h-4 w-4" />
            Delete
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>

      <EditTodoItemDialog open={showEditDialog} onOpenChange={setShowEditDialog} todoItem={todoItem} />
      <DeleteTodoItemDialog open={showDeleteDialog} onOpenChange={setShowDeleteDialog} todoItem={todoItem} />
    </>
  );
}
