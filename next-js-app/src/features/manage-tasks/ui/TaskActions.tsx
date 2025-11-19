"use client"

import { Button } from "@/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { TodoItem } from "@/entities/task/model/types"
import { MoreHorizontal, Pencil, Trash } from "lucide-react"
import { useState } from "react"
import { DeleteTaskDialog } from "./DeleteTaskDialog"
import { EditTaskDialog } from "./EditTaskDialog"

interface TaskActionsProps {
  task: TodoItem
}

export function TaskActions({ task }: TaskActionsProps) {
  const [showEditDialog, setShowEditDialog] = useState(false)
  const [showDeleteDialog, setShowDeleteDialog] = useState(false)

  return (
    <>
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button variant="ghost" className="h-8 w-8 p-0">
            <span className="sr-only">Open menu</span>
            <MoreHorizontal className="h-4 w-4" />
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align="end">
          <DropdownMenuItem onClick={() => setShowEditDialog(true)}>
            <Pencil className="mr-2 h-4 w-4" />
            Edit
          </DropdownMenuItem>
          <DropdownMenuItem onClick={() => setShowDeleteDialog(true)} className="text-red-600">
            <Trash className="mr-2 h-4 w-4" />
            Delete
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>

      <EditTaskDialog 
        open={showEditDialog} 
        onOpenChange={setShowEditDialog} 
        task={task} 
      />
      <DeleteTaskDialog 
        open={showDeleteDialog} 
        onOpenChange={setShowDeleteDialog} 
        task={task} 
      />
    </>
  )
}
