import { ArrowDown, ArrowRight, ArrowUp } from 'lucide-react';
import { DataTableFacetedFilter } from './FacetedFilter';

const PriorityOptions = [
  { label: 'High', value: 'high', icon: ArrowUp, className: 'text-red-500' },
  { label: 'Medium', value: 'medium', icon: ArrowRight, className: 'text-yellow-500' },
  { label: 'Low', value: 'low', icon: ArrowDown, className: 'text-green-500' }
];

export const PriorityFilter = () => {
  return <DataTableFacetedFilter title="Priority" options={PriorityOptions} />;
};
