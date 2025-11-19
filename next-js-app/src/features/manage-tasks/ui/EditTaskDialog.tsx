"use client"

import { Button } from "@/components/ui/button"
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import { useUpdateTask } from "@/entities/task/api/taskQueries"
import { TodoItem } from "@/entities/task/model/types"
import { zodResolver } from "@hookform/resolvers/zod"
import { useEffect } from "react"
import { useForm } from "react-hook-form"
import * as z from "zod"

const formSchema = z.object({
  title: z.string().min(1, "Title is required"),
  status: z.enum(["New", "InProgress", "Complete"]),
})

interface EditTaskDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  task: TodoItem
}

export function EditTaskDialog({ open, onOpenChange, task }: EditTaskDialogProps) {
  const updateTask = useUpdateTask()
  
  const { register, handleSubmit, reset, formState: { errors } } = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      title: task.title,
      status: task.status,
    },
  })

  useEffect(() => {
    if (open) {
      reset({
        title: task.title,
        status: task.status,
      })
    }
  }, [open, task, reset])

  const onSubmit = (values: z.infer<typeof formSchema>) => {
    updateTask.mutate({
      id: task.id,
      title: values.title,
      status: values.status,
    }, {
      onSuccess: () => {
        onOpenChange(false)
      },
    })
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Edit Task</DialogTitle>
          <DialogDescription>
            Make changes to your task here.
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit(onSubmit)} className="grid gap-4 py-4">
          <div className="grid gap-2">
            <label htmlFor="edit-title" className="text-sm font-medium leading-none">
              Title
            </label>
            <Input
              id="edit-title"
              {...register("title")}
            />
            {errors.title && (
              <p className="text-sm text-red-500">{errors.title.message}</p>
            )}
          </div>
          <div className="grid gap-2">
            <label htmlFor="edit-status" className="text-sm font-medium leading-none">
              Status
            </label>
            <select
              id="edit-status"
              className="flex h-10 w-full items-center justify-between rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 [&>span]:line-clamp-1"
              {...register("status")}
            >
              <option value="1">New</option>
              <option value="2">In Progress</option>
              <option value="3">Complete</option>
            </select>
            {errors.status && (
              <p className="text-sm text-red-500">{errors.status.message}</p>
            )}
          </div>
          <DialogFooter>
            <Button type="submit" disabled={updateTask.isPending}>
              {updateTask.isPending ? "Saving..." : "Save changes"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
