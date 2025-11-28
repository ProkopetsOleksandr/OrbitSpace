'use client';

import { goalsTestData } from '../GoalsPage';
import { GoalsTable } from './GoalsTable/GoalsTable';

export const GoalList = () => {
  return (
    <div>
      <GoalsTable data={goalsTestData} />
    </div>
  );
};
