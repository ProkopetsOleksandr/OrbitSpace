import { GoalList } from './GoalList/GoalList';

export type Goal = {
  id: string;
  title: string;
  description: string;
  completedDate: Date | null;
  createdAt: Date;
  status: 'New' | 'InProgress' | 'Completed' | 'OnHold';
  deadline: Date | null;
  category: string;
};

export const goalsTestData: Goal[] = [
  {
    id: '1',
    title: 'Learn TypeScript',
    description: 'Study the basics of TypeScript',
    completedDate: null,
    createdAt: new Date(),
    status: 'New',
    deadline: new Date(),
    category: 'Education'
  },
  {
    id: '2',
    title: 'Build a React App',
    description: 'Create a simple React application',
    completedDate: new Date(),
    createdAt: new Date(),
    status: 'InProgress',
    deadline: null,
    category: 'Development'
  }
];

export const GoalsPage = () => {
  return (
    <div>
      <GoalList />
    </div>
  );
};
