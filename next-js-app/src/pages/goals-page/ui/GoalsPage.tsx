import { GoalList } from '@/widgets/goals/goal-list';
import { GoalsStatusCards } from '@/widgets/goals/goals-stats';
import { GoalsToolbar } from '@/widgets/goals/goals-toolbar';

export const GoalsPage = () => {
  return (
    <div className="space-y-4">
      <GoalsStatusCards />
      <GoalsToolbar />
      <GoalList />
    </div>
  );
};
