'use client';

import { useQuery } from '@tanstack/react-query';
import { createColumnHelper, flexRender, getCoreRowModel, useReactTable } from '@tanstack/react-table';
import { useMemo } from 'react';

type TaskItem = {
  id: number;
  title: string;
  createdAt: Date;
  status: 'New' | 'In Progress' | 'Completed';
  timeSpentInMin: number;
  tag: string;
  taskType: 'Pomodoro' | 'Fixed time' | 'Regular';
};

const defaultData: TaskItem[] = [
  {
    id: 1,
    title: 'Read documentation on Next.js i18n',
    createdAt: new Date('2025-09-01T09:15:00'),
    status: 'Completed',
    timeSpentInMin: 45,
    tag: 'Learning',
    taskType: 'Pomodoro'
  },
  {
    id: 2,
    title: 'Implement login feature',
    createdAt: new Date('2025-09-02T10:30:00'),
    status: 'In Progress',
    timeSpentInMin: 120,
    tag: 'Work',
    taskType: 'Regular'
  },
  {
    id: 3,
    title: 'Gym workout',
    createdAt: new Date('2025-09-03T07:00:00'),
    status: 'Completed',
    timeSpentInMin: 90,
    tag: 'Health',
    taskType: 'Fixed time'
  },
  {
    id: 4,
    title: 'Prepare Docker environment',
    createdAt: new Date('2025-09-04T11:45:00'),
    status: 'In Progress',
    timeSpentInMin: 60,
    tag: 'DevOps',
    taskType: 'Pomodoro'
  },
  {
    id: 5,
    title: 'Walk with Jessie',
    createdAt: new Date('2025-09-05T08:15:00'),
    status: 'Completed',
    timeSpentInMin: 40,
    tag: 'Personal',
    taskType: 'Fixed time'
  },
  {
    id: 6,
    title: 'Write daily reflection in Notion',
    createdAt: new Date('2025-09-06T21:00:00'),
    status: 'New',
    timeSpentInMin: 0,
    tag: 'Self-development',
    taskType: 'Regular'
  },
  {
    id: 7,
    title: 'Fix bug in API response',
    createdAt: new Date('2025-09-07T14:20:00'),
    status: 'New',
    timeSpentInMin: 0,
    tag: 'Work',
    taskType: 'Regular'
  },
  {
    id: 8,
    title: 'Read 20 pages of book',
    createdAt: new Date('2025-09-08T22:00:00'),
    status: 'Completed',
    timeSpentInMin: 35,
    tag: 'Learning',
    taskType: 'Pomodoro'
  },
  {
    id: 9,
    title: 'Prepare weekly meal plan',
    createdAt: new Date('2025-09-09T17:10:00'),
    status: 'In Progress',
    timeSpentInMin: 25,
    tag: 'Health',
    taskType: 'Regular'
  },
  {
    id: 10,
    title: 'Team meeting',
    createdAt: new Date('2025-09-10T15:00:00'),
    status: 'Completed',
    timeSpentInMin: 50,
    tag: 'Work',
    taskType: 'Fixed time'
  }
];

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
