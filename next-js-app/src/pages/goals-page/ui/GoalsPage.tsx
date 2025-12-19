import { GoalsList } from '@/widgets/goals-list';
import { GoalsStatusCards } from '@/widgets/goals-stats';
import { GoalsToolbar } from '@/widgets/goals-toolbar';

export const GoalsPage = () => {
  return (
    <div className="space-y-4">
      <GoalsStatusCards />
      <GoalsToolbar />
      <GoalsList />
    </div>
  );
};
