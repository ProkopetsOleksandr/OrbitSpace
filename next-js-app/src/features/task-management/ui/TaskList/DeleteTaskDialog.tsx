'use client';

import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { TodoItem } from '@/entities/todoItem/model/types';
import { useDeleteTodoItem } from '../../api/todoItemQueries';

interface DeleteTaskDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  todoItem: TodoItem;
}

export function DeleteTaskDialog({ open, onOpenChange, todoItem }: DeleteTaskDialogProps) {
  const deleteTodoItem = useDeleteTodoItem();

  const onDelete = () => {
    deleteTodoItem.mutate(todoItem.id, {
      onSuccess: () => {
        onOpenChange(false);
      }
    });
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Delete Task</DialogTitle>
          <DialogDescription>
            Are you sure you want to delete task "{todoItem.title}"? This action cannot be undone.
          </DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)} disabled={deleteTodoItem.isPending}>
            Cancel
          </Button>
          <Button variant="destructive" onClick={onDelete} disabled={deleteTodoItem.isPending}>
            {deleteTodoItem.isPending ? 'Deleting...' : 'Delete'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
