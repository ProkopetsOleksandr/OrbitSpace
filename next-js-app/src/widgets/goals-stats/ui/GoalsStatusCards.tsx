import { GoalStatusCard } from '@/entities/goal';
import { CheckCircle2, PauseCircle, Target, TrendingUp } from 'lucide-react';

export const GoalsStatusCards = () => {
  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
      <GoalStatusCard title="Active" count={5} icon={<TrendingUp className="h-4 w-4" />} variant="active" />
      <GoalStatusCard title="On Hold" count={4} icon={<PauseCircle className="h-4 w-4" />} variant="onHold" />
      <GoalStatusCard title="New Goals" count={15} icon={<Target className="h-4 w-4" />} variant="new" />
      <GoalStatusCard title="Completed" count={27} icon={<CheckCircle2 className="h-4 w-4" />} variant="completed" />
    </div>
  );
};
