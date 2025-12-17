'use client';

import { CreateGoalDialog } from '@/features/create-goal';
import { Button } from '@/shared/ui/button';
import { Input } from '@/shared/ui/input';
import { ArrowDown, ArrowRight, ArrowUp, CheckCircle2, Circle, PauseCircle, Plus, Timer, XCircle } from 'lucide-react';
import { useState } from 'react';
import { DataTableFacetedFilter } from './FacetedFilter';

const PriorityOptions = [
  { label: 'High', value: 'high', icon: ArrowUp, className: 'text-red-500' },
  { label: 'Medium', value: 'medium', icon: ArrowRight, className: 'text-yellow-500' },
  { label: 'Low', value: 'low', icon: ArrowDown, className: 'text-green-500' }
];

const GoalStatusOptions = [
  { label: 'New', value: 'New', icon: Circle },
  { label: 'Active', value: 'Active', icon: Timer },
  { label: 'On Hold', value: 'OnHold', icon: PauseCircle },
  { label: 'Completed', value: 'Completed', icon: CheckCircle2 },
  { label: 'Cancelled', value: 'Cancelled', icon: XCircle }
];

export const Toolbar = () => {
  var [titleFilter, setTitleFilter] = useState<string>();

  return (
    <div className="flex items-center justify-between">
      <div className="flex flex-1 items-center space-x-2">
        <div>
          <Input
            placeholder="Filter goals..."
            value={titleFilter}
            onChange={event => setTitleFilter(event.target.value)}
            className="h-8 w-[150px] lg:w-[250px]"
          />
        </div>
        <div className="space-x-2">
          <DataTableFacetedFilter title="Status" options={GoalStatusOptions} />
          <DataTableFacetedFilter title="Priority" options={PriorityOptions} />
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
