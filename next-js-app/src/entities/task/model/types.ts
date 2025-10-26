export interface TaskItem {
  id: number;
  title: string;
  createdAt: Date;
  status: 'New' | 'In Progress' | 'Completed';
  timeSpentInMin: number;
  tag: string;
  taskType: 'Pomodoro' | 'Fixed time' | 'Regular';
}
