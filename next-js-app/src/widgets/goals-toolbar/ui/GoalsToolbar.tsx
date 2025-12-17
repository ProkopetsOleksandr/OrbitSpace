import { Plus } from 'lucide-react';

import { CreateGoalDialog } from '@/features/create-goal';
import { PriorityFilter, SearchInput, StatusFilter } from '@/features/filter-goals';
import { Button } from '@/shared/ui/button';

export const GoalsToolbar = () => {
  return (
    <div className="flex items-center justify-between">
      <div className="flex flex-1 items-center space-x-2">
        <div>
          <SearchInput />
        </div>
        <div className="space-x-2">
          <StatusFilter />
          <PriorityFilter />
        </div>
      </div>
      <div>
        <CreateGoalDialog>
          <Button className="cursor-pointer">
            <Plus className="mr-2 h-4 w-4" />
            Add Goal
          </Button>
        </CreateGoalDialog>
      </div>
    </div>
  );
};
