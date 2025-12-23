import { Fragment, ReactNode } from 'react';

import { DayProgress } from '@/widgets/day-planner/day-progress';
import { TodoItemList } from '@/widgets/todo-items/todo-item-list';

const WidgetCard = ({ children }: { children: ReactNode }) => {
  return <div className="p-4 bg-gray-50 rounded-lg shadow-sm">{children}</div>;
};

export const TaskManagementPage = () => {
  return (
    <Fragment>
      <div className="grid gap-4 grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 mb-4">
        <WidgetCard>Card1</WidgetCard>
        <WidgetCard>Card2</WidgetCard>
        <WidgetCard>
          <DayProgress />
        </WidgetCard>
      </div>
      <WidgetCard>
        <TodoItemList />
      </WidgetCard>
    </Fragment>
  );
};
