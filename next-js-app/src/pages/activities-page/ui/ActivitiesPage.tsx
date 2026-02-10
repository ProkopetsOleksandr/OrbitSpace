import { Settings } from 'lucide-react';

import { Button } from '@/shared/ui/button';
import { ManageActivitiesDialog } from '@/widgets/activities/manage-activities';

export const ActivitiesPage = () => {
  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">Activities</h1>
        <ManageActivitiesDialog>
          <Button variant="outline" className="cursor-pointer">
            <Settings className="mr-2 h-4 w-4" />
            Manage Activities
          </Button>
        </ManageActivitiesDialog>
      </div>
    </div>
  );
};
