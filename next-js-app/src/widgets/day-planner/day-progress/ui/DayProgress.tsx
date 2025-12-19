'use client';

import { useEffect, useState } from 'react';

import { DayProgressBar } from '@/shared/ui/day-progress-bar';

export const DayProgress = () => {
  const [now, setNow] = useState(new Date());

  useEffect(() => {
    const id = setInterval(() => setNow(new Date()), 10000); // 10 сек для плавности
    return () => clearInterval(id);
  }, []);

  const startTime = 8;
  const endTime = 22;

  return (
    <div className="p-4 bg-white rounded-xl shadow-sm">
      <h2 className="text-lg font-bold mb-4">{now.toDateString()}</h2>
      <DayProgressBar startTimeHour={startTime} endTimeHour={endTime} currentTime={now} />
    </div>
  );
};
