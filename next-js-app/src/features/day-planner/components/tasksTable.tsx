'use client';

import { flexRender, getCoreRowModel, useReactTable } from '@tanstack/react-table';
import { useTasks } from '../hooks/useTasks';
import { taskTableColumns } from './taskTableColumns';

export default function TasksTable() {
  const { data: tasks = [], isLoading } = useTasks();

  const table = useReactTable({
    data: tasks,
    columns: taskTableColumns,
    getCoreRowModel: getCoreRowModel()
  });

  return (
    <div>
      {isLoading && <div>Data is loading...</div>}
      <table className="min-w-full divide-y divide-gray-200 border border-gray-300">
        <thead>
          {table.getHeaderGroups().map(headerGroup => (
            <tr key={headerGroup.id}>
              {headerGroup.headers.map(header => (
                <th
                  key={header.id}
                  className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider bg-gray-50">
                  {header.isPlaceholder ? null : <div>{flexRender(header.column.columnDef.header, header.getContext())}</div>}
                </th>
              ))}
            </tr>
          ))}
        </thead>
        <tbody className="bg-white divide-y divide-gray-200">
          {table.getRowModel().rows.map(row => (
            <tr key={row.id} className="hover:bg-gray-100">
              {row.getVisibleCells().map(cell => (
                <td key={cell.id} className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {flexRender(cell.column.columnDef.cell, cell.getContext())}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
