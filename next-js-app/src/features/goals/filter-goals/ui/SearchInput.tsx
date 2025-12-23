'use client';

import { Input } from '@/shared/ui/input';
import { useGoalFilters } from '../model/use-goal-filters';

export const SearchInput = () => {
  const { filters, setFilters } = useGoalFilters();

  return (
    <Input
      placeholder="Filter goals..."
      value={filters.search ?? ''}
      onChange={event => setFilters({ search: event.target.value })}
      className="h-8 w-[150px] lg:w-[250px]"
    />
  );
};
