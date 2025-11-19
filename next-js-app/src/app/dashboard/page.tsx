import { TasksList } from "@/features/manage-tasks/ui/TasksList"

export default function DashboardPage() {
  return (
    <div className="h-full flex-1 flex-col space-y-8 p-8 md:flex">
      <div className="flex items-center justify-between space-y-2">
        <div>
          <h2 className="text-2xl font-bold tracking-tight">Dashboard</h2>
          <p className="text-muted-foreground">
            Manage your tasks and view their status.
          </p>
        </div>
      </div>
      <TasksList />
    </div>
  )
}
