import type { paths } from '@/types/api-types';

export type TaskItem = paths['/api/todo-items']['get']['responses']['200']['content']['application/json']['data'][number];
