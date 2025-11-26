'use client';

import { Button } from '@/components/ui/button';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from '@/components/ui/dropdown-menu';
import { TodoItem } from '@/entities/todoItem/model/types';
import { MoreHorizontal, Pencil, Trash } from 'lucide-react';
import { useState } from 'react';
import { DeleteTaskDialog } from './DeleteTaskDialog';
import { EditTaskDialog } from './EditTaskDialog';

interface TaskTableActionsProps {
  todoItem: TodoItem;
}

export function TaskTableActions({ todoItem }: TaskTableActionsProps) {
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

      <EditTaskDialog open={showEditDialog} onOpenChange={setShowEditDialog} todoItem={todoItem} />
      <DeleteTaskDialog open={showDeleteDialog} onOpenChange={setShowDeleteDialog} todoItem={todoItem} />
    </>
  );
}
