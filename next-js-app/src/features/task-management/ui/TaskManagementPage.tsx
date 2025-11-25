import { Fragment } from 'react';
import { WidgetCard } from './Card';
import { DayProgressBar } from './DayProgressBar';
import { TaskList } from './TaskList/TaskList';

export const TaskManagementPage = () => {
  return (
    <Fragment>
      <div className="grid auto-rows-min gap-4 md:grid-cols-3">
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
