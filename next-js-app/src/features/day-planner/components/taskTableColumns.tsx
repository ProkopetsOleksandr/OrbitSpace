import { TaskItem } from '@/entities/task/model/types';
import { createColumnHelper } from '@tanstack/react-table';

const columnHelper = createColumnHelper<TaskItem>();

export const taskTableColumns = [
  columnHelper.accessor('title', {
    header: () => 'Title',
    cell: info => info.getValue()
  }),

  columnHelper.accessor('taskType', {
    header: 'Task Type',
    cell: info => info.getValue()
  }),

  columnHelper.accessor('createdAt', {
    header: 'Created At',
    cell: info => info.getValue().toLocaleDateString()
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
    header: 'Action',
    cell: () => <button>Редактировать</button>
  })
];
