import { Fragment } from 'react';

import { WidgetCard } from '@/features/task-management/ui/Card';
import { DayProgressBar } from '@/features/task-management/ui/DayProgressBar';
import { TaskList } from '@/features/task-management/ui/TaskList/TaskList';

export const TaskManagementPage = () => {
  return (
    <Fragment>
      <div className="grid gap-4 grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 mb-4">
        <WidgetCard>Card1</WidgetCard>
        <WidgetCard>Card2</WidgetCard>
        <WidgetCard>
          <DayProgressBar startTimeHour={6} endTimeHour={22} />
        </WidgetCard>
      </div>
      <WidgetCard>
        <TaskList />
      </WidgetCard>
    </Fragment>
  );
};
