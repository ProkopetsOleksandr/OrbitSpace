'use client';

import { GoalsTable, useGoals } from '@/entities/goal';

export const GoalsList = () => {
  const { data: goals, isLoading, error } = useGoals();

  return <GoalsTable data={goals} />;
};
