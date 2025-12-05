'use client';

import { TodoItem } from '@/entities/todoItem/model/types';
import { createColumnHelper } from '@tanstack/react-table';
import { TaskTableActions } from './TaskTableActions';

const columnHelper = createColumnHelper<TodoItem>();

export const taskTableColumns = [
  columnHelper.accessor('title', {
    header: () => 'Title',
    cell: info => info.getValue()
  }),

  columnHelper.accessor('status', {
    header: 'Status',
    cell: ({ row }) => {
      const status = row.original.status;
      return (
        <span
          className={`px-2 py-1 rounded-full text-xs font-medium ${
            status === 'New'
              ? 'bg-blue-100 text-blue-800'
              : status === 'InProgress'
              ? 'bg-yellow-100 text-yellow-800'
              : 'bg-green-100 text-green-800'
          }`}>
          {status}
        </span>
      );
    }
  }),

  columnHelper.accessor('createdAtUtc', {
    header: 'Created At',
    cell: info => info.getValue()
  }),

  columnHelper.accessor('updatedAtUtc', {
    header: 'Updated At',
    cell: info => info.getValue()
  }),

  columnHelper.display({
    id: 'actions',
    cell: ({ row }) => {
      return <TaskTableActions todoItem={row.original} />;
    }
  })
];
