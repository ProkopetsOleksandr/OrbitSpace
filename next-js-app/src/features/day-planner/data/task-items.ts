export type TaskItem = {
  id: number;
  title: string;
  createdAt: Date;
  status: 'New' | 'In Progress' | 'Completed';
  timeSpentInMin: number;
  tag: string;
  taskType: 'Pomodoro' | 'Fixed time' | 'Regular';
};

export const defaultData: TaskItem[] = [
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
