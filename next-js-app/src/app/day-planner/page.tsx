import { ClipboardList } from 'lucide-react';
import React from 'react';
import Tasks from './tasks';

export default function Page() {
  return (
    <React.Fragment>
      <div className="grid auto-rows-min gap-4 md:grid-cols-3">
        <div className="p-4 bg-gray-50 rounded-lg shadow-sm">
          <div className="flex gap-2 items-center mb-5">
            <ClipboardList className="size-5" />
            <h2 className="font-semibold">Tasks</h2>
          </div>
          <Tasks />
        </div>
        <div className="p-4 bg-gray-50 rounded-lg shadow-sm"></div>
        <div className="p-4 bg-gray-50 rounded-lg shadow-sm"></div>
      </div>
    </React.Fragment>
  );
}
