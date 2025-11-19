"use client"

import { TodoItem } from "@/entities/task/model/types"
import { ColumnDef } from "@tanstack/react-table"
import { TaskActions } from "./TaskActions"

export const columns: ColumnDef<TodoItem>[] = [
  {
    accessorKey: "title",
    header: "Title",
  },
  {
    accessorKey: "status",
    header: "Status",
    cell: ({ row }) => {
      const status = row.original.status
      return (
        <span className={`px-2 py-1 rounded-full text-xs font-medium ${
          status === 'New' ? 'bg-blue-100 text-blue-800' :
          status === 'InProgress' ? 'bg-yellow-100 text-yellow-800' :
          'bg-green-100 text-green-800'
        }`}>
          {status}
        </span>
      )
    }
  },
  {
    accessorKey: "createdAt",
    header: "Created At",
    cell: ({ row }) => {
      return new Date(row.original.createdAt).toLocaleDateString()
    }
  },
  {
    id: "actions",
    cell: ({ row }) => <TaskActions task={row.original} />
  },
]
