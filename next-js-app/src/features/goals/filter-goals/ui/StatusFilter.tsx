'use client';

import { CheckCircle2, Circle, PauseCircle, Timer, XCircle } from 'lucide-react';

import { DataTableFacetedFilter } from './FacetedFilter';

const GoalStatusOptions = [
  { label: 'New', value: 'New', icon: Circle },
  { label: 'Active', value: 'Active', icon: Timer },
  { label: 'On Hold', value: 'OnHold', icon: PauseCircle },
  { label: 'Completed', value: 'Completed', icon: CheckCircle2 },
  { label: 'Cancelled', value: 'Cancelled', icon: XCircle }
];

export const StatusFilter = () => {
  return <DataTableFacetedFilter title="Status" options={GoalStatusOptions} />;
};
