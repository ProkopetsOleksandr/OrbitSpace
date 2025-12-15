import { GoalList } from '@/features/goals/ui/GoalList/GoalList';
import { StatusCards } from '@/features/goals/ui/GoalList/StatusCards';
import { Toolbar } from '@/features/goals/ui/Toolbar/Toolbar';

export const GoalsPage = () => {
  return (
    <div className="space-y-4">
      <StatusCards activeCount={6} onHoldCount={5} newCount={10} completedCount={221} />
      <Toolbar />
      <GoalList />
    </div>
  );
};
