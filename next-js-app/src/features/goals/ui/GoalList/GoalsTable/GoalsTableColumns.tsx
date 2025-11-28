'use client';

import { createColumnHelper } from '@tanstack/react-table';
import { Goal } from '../../GoalsPage';
import { GoalsTableRowActions } from './GoalsTableRowActions';

const columnHelper = createColumnHelper<Goal>();

export const columns = [
  columnHelper.accessor('title', {
    header: () => 'Title',
    cell: info => info.getValue()
  }),

  columnHelper.accessor('category', {
    header: () => 'Category',
    cell: info => info.getValue()
  }),

  columnHelper.accessor('createdAt', {
    header: () => 'Created At',
    cell: info => info.getValue().toLocaleDateString()
  }),

  columnHelper.accessor('status', {
    header: () => 'Status',
    cell: info => info.getValue()
  }),

  columnHelper.accessor('deadline', {
    header: () => 'Deadline',
    cell: info => {
      const value = info.getValue();
      return value ? value.toLocaleDateString() : 'N/A';
    }
  }),

  columnHelper.accessor('completedDate', {
    header: () => 'Completed Date',
    cell: info => {
      const value = info.getValue();
      return value ? value.toLocaleDateString() : 'N/A';
    }
  }),

  columnHelper.display({
    id: 'actions',
    cell: ({ row }) => <GoalsTableRowActions row={row.original} />
  })
];
