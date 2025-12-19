'use client';

import { GoalTable, useGoals } from '@/entities/goal';

export const GoalList = () => {
  const { data: goals, isLoading, error } = useGoals();

  return <GoalTable data={goals} />;
};
