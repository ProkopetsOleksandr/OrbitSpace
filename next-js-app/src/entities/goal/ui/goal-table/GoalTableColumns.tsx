'use client';

import { createColumnHelper } from '@tanstack/react-table';
import { formatDate } from 'date-fns';

import { Goal, GoalStatus } from '@/shared/api';
import { cn } from '@/shared/lib/utils';
import { Badge } from '@/shared/ui/badge';
import { GoalTableRowActions } from './GoalTableRowActions';

const statusStyles: Record<GoalStatus, string> = {
  [GoalStatus.NotStarted]: 'bg-slate-100 text-slate-700 border-slate-200',
  [GoalStatus.Active]: 'bg-blue-50 text-blue-700 border-blue-200',
  [GoalStatus.OnHold]: 'bg-orange-50 text-orange-700 border-orange-200',
  [GoalStatus.Completed]: 'bg-emerald-50 text-emerald-700 border-emerald-200',
  [GoalStatus.Cancelled]: 'bg-red-50 text-red-700 border-red-200'
};

const formatStatusLabel = (status: string) => {
  return status.replace(/([A-Z])/g, ' $1').trim();
};

const columnHelper = createColumnHelper<Goal>();

export const columns = [
  columnHelper.accessor('title', {
    header: 'Title',
    cell: ({ row }) => {
      return (
        <div>
          <span className="text-foreground font-semibold">{row.original.title}</span>
          {row.original.isSmartGoal && (
            <Badge variant="secondary" className="ml-2 text-xs">
              SMART
            </Badge>
          )}
        </div>
      );
    }
  }),

  columnHelper.accessor('status', {
    header: 'Status',
    cell: ({ row }) => {
      const status = row.original.status;
      const styles = statusStyles[status];

      return (
        <span className={cn('inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border', styles)}>
          {formatStatusLabel(status)}
        </span>
      );
    }
  }),

  columnHelper.accessor('lifeArea', {
    header: 'Category',
    cell: info => <span>{formatStatusLabel(info.getValue())}</span>
  }),

  columnHelper.accessor('createdAtUtc', {
    header: 'Created At',
    cell: info => {
      const date = info.getValue();
      return <span className="whitespace-nowrap">{formatDate(date, 'dd/MM/yyyy')}</span>;
    }
  }),

  columnHelper.accessor('completedAtUtc', {
    header: 'Completed At',
    cell: info => {
      const date = info.getValue();

      return date ? (
        <span className="whitespace-nowrap">{formatDate(date, 'dd/MM/yyyy')}</span>
      ) : (
        <span className="text-muted-foreground text-xs">Not completed</span>
      );
    }
  }),

  columnHelper.accessor('dueAtUtc', {
    header: 'Deadline',
    cell: info => {
      const date = info.getValue();
      return date ? (
        <span className="whitespace-nowrap">{formatDate(date, 'dd/MM/yyyy')}</span>
      ) : (
        <span className="text-muted-foreground text-xs">No deadline</span>
      );
    }
  }),

  columnHelper.display({
    id: 'actions',
    cell: ({ row }) => (
      <div className="flex justify-end">
        <GoalTableRowActions row={row.original} />
      </div>
    ),
    enableSorting: false,
    enableHiding: false
  })
];
