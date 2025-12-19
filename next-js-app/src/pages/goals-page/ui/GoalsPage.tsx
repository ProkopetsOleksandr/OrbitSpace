import { GoalsList, GoalsStatusCards, GoalsToolbar } from '@/widgets/goals';

export const GoalsPage = () => {
  return (
    <div className="space-y-4">
      <GoalsStatusCards />
      <GoalsToolbar />
      <GoalsList />
    </div>
  );
};
