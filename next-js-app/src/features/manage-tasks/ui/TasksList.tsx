"use client"

import { useTasks } from "@/entities/task/api/taskQueries"
import { Loader2 } from "lucide-react"
import { CreateTaskDialog } from "./CreateTaskDialog"
import { TaskTable } from "./TaskTable"

export function TasksList() {
  const { data: tasks, isLoading, error } = useTasks()

  if (isLoading) {
    return (
      <div className="flex h-full items-center justify-center p-8">
        <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
      </div>
    )
  }

  if (error) {
    return (
      <div className="flex h-full items-center justify-center p-8 text-red-500">
        Error loading tasks: {error.message}
      </div>
    )
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-bold tracking-tight">Tasks</h2>
        <CreateTaskDialog />
      </div>
      <TaskTable data={tasks || []} />
    </div>
  )
}
