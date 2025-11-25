import { Fragment } from 'react';
import { Card } from './Card';
import { DayProgressBar } from './DayProgressBar';

export const TaskManagementPage = () => {
  return (
    <Fragment>
      <div className="grid auto-rows-min gap-4 md:grid-cols-3">
        <Card>Card1</Card>
        <Card>Card2</Card>
        <Card>
          <DayProgressBar startTimeHour={6} endTimeHour={22} />
        </Card>
      </div>
      <Card>Card4</Card>
    </Fragment>
  );
};
