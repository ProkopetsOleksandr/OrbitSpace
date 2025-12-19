'use client';

import { flexRender, getCoreRowModel, Table, useReactTable } from '@tanstack/react-table';
import { Loader2 } from 'lucide-react';

import { TodoItem } from '@/shared/api';
import { taskTableColumns } from './TodoItemTableColumns';

interface TodoItemsTableProps {
  isLoading: boolean;
  data: TodoItem[];
  error: Error | null;
}

interface TodoItemsTableBodyProps {
  table: Table<TodoItem>;
  isLoading: boolean;
  error: Error | null;
}

const TodoItemTableBody = ({ table, isLoading, error }: TodoItemsTableBodyProps) => {
  if (isLoading) {
    return (
      <tbody>
        <tr>
          <td colSpan={taskTableColumns.length} className="h-24 text-center">
            <div className="flex items-center justify-center gap-4">
              <Loader2 className="h-8 w-8 animate-spin text-blue-500" />
              Loading...
            </div>
          </td>
        </tr>
      </tbody>
    );
  }

  if (error) {
    return (
      <tbody>
        <tr>
          <td colSpan={taskTableColumns.length} className="h-24 text-center">
            Something went wrong. Details: {error.message}
          </td>
        </tr>
      </tbody>
    );
  }

  if (!table.getRowModel().rows?.length) {
    return (
      <tbody>
        <tr>
          <td colSpan={taskTableColumns.length} className="h-24 text-center">
            No tasks yet. Create one to get started!
          </td>
        </tr>
      </tbody>
    );
  }

  return (
    <tbody className="[&_tr:last-child]:border-0">
      {table.getRowModel().rows.map(row => (
        <tr
          key={row.id}
          data-state={row.getIsSelected() && 'selected'}
          className="border-b transition-colors hover:bg-muted/50 data-[state=selected]:bg-muted">
          {row.getVisibleCells().map(cell => (
            <td key={cell.id} className="p-4 align-middle [&:has([role=checkbox])]:pr-0">
              {flexRender(cell.column.columnDef.cell, cell.getContext())}
            </td>
          ))}
        </tr>
      ))}
    </tbody>
  );
};

export const TodoItemTable = ({ isLoading, data, error }: TodoItemsTableProps) => {
  const table = useReactTable({
    data,
    columns: taskTableColumns,
    getCoreRowModel: getCoreRowModel()
  });

  return (
    <div className="rounded-md border">
      <div className="w-full overflow-auto">
        <table className="w-full caption-bottom text-sm">
          <thead className="[&_tr]:border-b">
            {table.getHeaderGroups().map(headerGroup => (
              <tr key={headerGroup.id} className="border-b transition-colors hover:bg-muted/50 data-[state=selected]:bg-muted">
                {headerGroup.headers.map(header => {
                  return (
                    <th
                      key={header.id}
                      className="h-12 px-4 text-left align-middle font-medium text-muted-foreground [&:has([role=checkbox])]:pr-0">
                      {header.isPlaceholder ? null : flexRender(header.column.columnDef.header, header.getContext())}
                    </th>
                  );
                })}
              </tr>
            ))}
          </thead>
          <TodoItemTableBody table={table} isLoading={isLoading} error={error} />
        </table>
      </div>
    </div>
  );
};
