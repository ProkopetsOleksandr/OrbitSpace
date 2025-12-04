'use client';

import { createColumnHelper } from '@tanstack/react-table';

import { Goal, GoalStatus } from '@/entities/goal/model/types';
import { formatDate } from 'date-fns';
import { GoalsTableRowActions } from './GoalsTableRowActions';

const columnHelper = createColumnHelper<Goal>();

export const columns = [
  columnHelper.accessor('title', {
    header: () => 'Title',
    cell: info => info.getValue()
  }),

  columnHelper.accessor('status', {
    header: 'Status',
    cell: ({ row }) => {
      const status = row.original.status;

      const statusClasses =
        status === GoalStatus.NotStarted
          ? 'bg-blue-100 text-blue-800'
          : status === GoalStatus.Active
          ? 'bg-yellow-100 text-yellow-800'
          : status === GoalStatus.Completed
          ? 'bg-green-100 text-green-800'
          : status === GoalStatus.OnHold
          ? 'bg-gray-200 text-gray-700'
          : '';

      return (
        <span
          className={`
          px-2 py-1 rounded-full text-xs font-medium
          ${statusClasses}
        `}>
          {status.replace(/([A-Z])/g, ' $1').trim()}
        </span>
      );
    }
  }),

  columnHelper.accessor('lifeArea', {
    header: () => 'Category',
    cell: info => info.getValue()
  }),

  columnHelper.accessor('createdAt', {
    header: () => 'Created At',
    cell: info => formatDate(info.getValue(), 'MM/dd/yyyy')
  }),

  columnHelper.accessor('dueDate', {
    header: () => 'Deadline',
    cell: info => {
      const value = info.getValue();
      return value ? formatDate(value, 'MM/dd/yyyy') : 'N/A';
    }
  }),

  columnHelper.accessor('completedDate', {
    header: () => 'Completed Date',
    cell: info => {
      const value = info.getValue();
      return value ? formatDate(value, 'MM/dd/yyyy') : 'N/A';
    }
  }),

  columnHelper.display({
    id: 'actions',
    cell: ({ row }) => <GoalsTableRowActions row={row.original} />
  })
];
