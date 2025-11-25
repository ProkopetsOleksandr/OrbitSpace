import React from 'react';

export default function TaskManagementPage() {
  return (
    <React.Fragment>
      <div className="grid auto-rows-min gap-4 md:grid-cols-3">
        <div className="p-4 bg-gray-50 rounded-lg shadow-sm"></div>
        <div className="p-4 bg-gray-50 rounded-lg shadow-sm"></div>
        <div className="p-4 bg-gray-50 rounded-lg shadow-sm"></div>
      </div>
      <div className="p-4 bg-gray-50 rounded-lg shadow-sm"></div>
    </React.Fragment>
  );
}
