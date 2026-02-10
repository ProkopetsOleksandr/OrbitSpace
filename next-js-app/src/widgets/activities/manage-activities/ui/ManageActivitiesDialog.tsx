'use client';

import { zodResolver } from '@hookform/resolvers/zod';
import { ArrowLeft, Plus } from 'lucide-react';
import { useState } from 'react';
import { useForm } from 'react-hook-form';

import { ActivityTable, useActivities } from '@/entities/activity';
import {
  CreateActivityForm,
  createActivitySchema,
  type CreateActivityFormValues,
  useCreateActivity
} from '@/features/activities/create-activity';
import { Button } from '@/shared/ui/button';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger
} from '@/shared/ui/dialog';

type DialogView = 'list' | 'create';

export function ManageActivitiesDialog({ children }: { children: React.ReactNode }) {
  const [open, setOpen] = useState(false);
  const [view, setView] = useState<DialogView>('list');

  const { data: activities } = useActivities();
  const { mutate: createActivity, isPending } = useCreateActivity();

  const form = useForm<CreateActivityFormValues>({
    resolver: zodResolver(createActivitySchema),
    defaultValues: {
      name: '',
      code: ''
    }
  });

  const handleOpenChange = (newOpenState: boolean) => {
    if (newOpenState) {
      setOpen(true);
      setView('list');
      form.reset();
      return;
    }

    if (view === 'create' && form.formState.isDirty) {
      const confirmClose = window.confirm('You have unsaved changes. Are you sure you want to close?');
      if (!confirmClose) return;
    }

    setOpen(false);
    setView('list');
  };

  const handleBack = () => {
    if (form.formState.isDirty) {
      const confirmBack = window.confirm('You have unsaved changes. Are you sure you want to go back?');
      if (!confirmBack) return;
    }

    form.reset();
    setView('list');
  };

  const onSubmit = (values: CreateActivityFormValues) => {
    createActivity(values, {
      onSuccess: () => {
        form.reset();
        setView('list');
      }
    });
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogTrigger asChild>{children}</DialogTrigger>
      <DialogContent className="sm:max-w-lg">
        {view === 'list' && (
          <>
            <DialogHeader>
              <div className="flex items-center justify-between">
                <div>
                  <DialogTitle>Manage Activities</DialogTitle>
                  <DialogDescription>Your activity actions for the Activity Grid.</DialogDescription>
                </div>
                <Button size="sm" className="cursor-pointer" onClick={() => setView('create')}>
                  <Plus className="mr-1 h-4 w-4" />
                  Add new
                </Button>
              </div>
            </DialogHeader>
            <ActivityTable data={activities} />
          </>
        )}

        {view === 'create' && (
          <>
            <DialogHeader>
              <div className="flex items-center gap-2">
                <Button variant="ghost" size="icon" className="cursor-pointer" onClick={handleBack}>
                  <ArrowLeft className="h-4 w-4" />
                </Button>
                <div>
                  <DialogTitle>New Activity</DialogTitle>
                  <DialogDescription>Create a new activity to track.</DialogDescription>
                </div>
              </div>
            </DialogHeader>
            <CreateActivityForm id="create-activity-form" form={form} onSubmit={onSubmit} />
            <DialogFooter>
              <Button type="submit" form="create-activity-form" disabled={isPending}>
                {isPending ? 'Saving...' : 'Create'}
              </Button>
            </DialogFooter>
          </>
        )}
      </DialogContent>
    </Dialog>
  );
}
