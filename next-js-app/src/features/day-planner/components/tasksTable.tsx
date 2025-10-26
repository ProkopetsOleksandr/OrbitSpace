'use client';

import { defaultData, type TaskItem } from '@/features/day-planner/data/task-items';
import { useQuery } from '@tanstack/react-query';
import { createColumnHelper, flexRender, getCoreRowModel, useReactTable } from '@tanstack/react-table';
import { useMemo } from 'react';

const columnHelper = createColumnHelper<TaskItem>();

const columns = [
  columnHelper.accessor('title', {
    header: () => 'Title', // Заголовок
    cell: info => info.getValue()
  }),

  columnHelper.accessor('taskType', {
    header: 'Task Type',
    cell: info => info.getValue()
  }),

  columnHelper.accessor('createdAt', {
    header: 'Created At',
    cell: info => info.getValue().toLocaleDateString() // Например, курсивом
  }),

  columnHelper.accessor('status', {
    header: 'Status',
    cell: info => info.getValue()
  }),

  columnHelper.accessor('timeSpentInMin', {
    header: 'Time Spent',
    cell: info => info.getValue()
  }),

  columnHelper.accessor('tag', {
    header: 'Tag',
    cell: info => info.getValue()
  }),

  columnHelper.display({
    id: 'actions',
    header: 'Действия',
    cell: () => <button>Редактировать</button>
  })
];

async function getTasks(): Promise<TaskItem[]> {
  return new Promise(resolve =>
    setTimeout(function () {
      return resolve(defaultData);
    }, 1000)
  );
}

export default function TasksTable() {
  const query = useQuery({
    queryKey: ['tasks'],
    queryFn: getTasks
  });

  const tableData = useMemo(() => (query.data ? query.data : []), [query.data]);

  const table = useReactTable({
    data: tableData, // 1. Ваши данные
    columns: columns, // 2. Ваша конфигурация колонок
    getCoreRowModel: getCoreRowModel() // 3. Обязательный плагин для получения базовой модели
  });

  return (
    <div>
      {query.isLoading && <div>Data is loading...</div>}
      <table className="min-w-full divide-y divide-gray-200 border border-gray-300">
        <thead>
          {/* Получаем все группы строк заголовков */}
          {table.getHeaderGroups().map(headerGroup => (
            <tr key={headerGroup.id}>
              {/* Рендерим заголовки для каждой колонки */}
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
          {/* Получаем все строки тела таблицы */}
          {table.getRowModel().rows.map(row => (
            <tr key={row.id} className="hover:bg-gray-100">
              {/* Рендерим ячейки для каждой строки */}
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
