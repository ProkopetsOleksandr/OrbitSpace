import { GoalList } from './GoalList/GoalList';
import { StatusCards } from './GoalList/StatusCards';
import { Toolbar } from './Toolbar/Toolbar';

export const GoalsPage = () => {
  return (
    <div className="space-y-4">
      <StatusCards activeCount={6} onHoldCount={5} newCount={10} completedCount={221} />
      <Toolbar />
      <GoalList />
    </div>
  );
};
