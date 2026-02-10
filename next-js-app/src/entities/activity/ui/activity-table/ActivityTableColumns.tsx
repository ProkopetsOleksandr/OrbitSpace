'use client';

import { createColumnHelper } from '@tanstack/react-table';

import { Activity } from '@/shared/api';
import { Badge } from '@/shared/ui/badge';

const columnHelper = createColumnHelper<Activity>();

export const columns = [
  columnHelper.accessor('name', {
    header: 'Name',
    cell: ({ row }) => {
      return <span className="text-foreground font-semibold">{row.original.name}</span>;
    }
  }),

  columnHelper.accessor('code', {
    header: 'Code',
    cell: info => (
      <Badge variant="secondary" className="font-mono text-xs">
        {info.getValue()}
      </Badge>
    )
  })
];
