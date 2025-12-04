'use client';

import { useGoals } from '../../api/goal-queries';
import { GoalsTable } from './GoalsTable/GoalsTable';

export const GoalList = () => {
  const { data: goals, isLoading, error } = useGoals();

  return (
    <div>
      <GoalsTable data={goals} />
    </div>
  );
};
