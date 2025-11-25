import { TaskTable } from './TaskTable';

export const TaskList = () => {
  return (
    <div>
      <div className="flex items-center justify-between space-y-2 mb-4">
        <div>
          <h2 className="text-2xl font-bold tracking-tight">Tasks</h2>
          <p className="text-muted-foreground">Manage your tasks and view their status.</p>
        </div>
      </div>
      <TaskTable
        data={[
          {
            id: '1',
            title: 'Sample Task',
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString(),
            status: 'New'
          }
        ]}
      />
    </div>
  );
};
