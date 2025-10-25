import TasksTable from '@/features/day-planner/components/tasksTable';
import TimeSegmentDisplay from '@/features/day-planner/components/TimeSegment';
import { ClipboardList } from 'lucide-react';
import React from 'react';
import LogoutBtb from '../../features/day-planner/components/LogoutBtb';

export default function Page() {
  return (
    <React.Fragment>
      <div className="grid auto-rows-min gap-4 md:grid-cols-3">
        <div className="p-4 bg-gray-50 rounded-lg shadow-sm">
          <div className="flex gap-2 items-center mb-5">
            <ClipboardList className="size-5" />
            <h2 className="font-semibold">Tasks</h2>
          </div>
          <LogoutBtb />
        </div>
        <div className="p-4 bg-gray-50 rounded-lg shadow-sm">
          <TimeSegmentDisplay startTimeHour={6} endTimeHour={22} />
        </div>
        <div className="p-4 bg-gray-50 rounded-lg shadow-sm"></div>
      </div>
      <div className="p-4 bg-gray-50 rounded-lg shadow-sm">
        <div className="flex gap-2 items-center mb-5">
          <ClipboardList className="size-5" />
          <h2 className="font-semibold">Tasks</h2>
        </div>
        <TasksTable />
      </div>
    </React.Fragment>
  );
}
