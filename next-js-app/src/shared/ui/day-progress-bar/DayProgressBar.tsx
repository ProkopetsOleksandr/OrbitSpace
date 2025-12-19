'use client';

import { useMemo } from 'react';
import { calculatePositionPercentage } from './calc-progress';

interface DayProgressBarProps {
  startTimeHour: number;
  endTimeHour: number;
  currentTime: Date;
  className?: string;
}

export const DayProgressBar = ({ startTimeHour, endTimeHour, currentTime, className }: DayProgressBarProps) => {
  const startMinutes = startTimeHour * 60;
  const endMinutes = endTimeHour * 60;
  const currentMinutes = currentTime.getHours() * 60 + currentTime.getMinutes();

  const positionPercentage = useMemo(() => {
    return calculatePositionPercentage(currentMinutes, startMinutes, endMinutes);
  }, [currentMinutes, startMinutes, endMinutes]);

  const formatTime = (hour: number) => {
    const period = hour >= 12 && hour !== 24 ? 'PM' : 'AM';
    const displayHour = hour % 12 === 0 ? (hour === 24 ? 0 : 12) : hour % 12;
    return `${displayHour.toString().padStart(2, '0')}:00 ${period}`;
  };

  return (
    <div className={className}>
      <div className="flex justify-between text-sm font-medium text-gray-600 mb-2">
        <span>{formatTime(startTimeHour)}</span>
        <span>{formatTime(endTimeHour)}</span>
      </div>

      <div className="relative h-2 bg-indigo-200 rounded-full">
        {/* Прогресс */}
        <div
          className="absolute top-0 left-0 h-full bg-indigo-600 rounded-full transition-all duration-500"
          style={{ width: `${positionPercentage}%` }}
        />

        {/* Точка "Сейчас" */}
        <div
          className="absolute top-1/2 -translate-y-1/2 -translate-x-1/2 w-3 h-3 bg-indigo-800 rounded-full shadow-md z-10 transition-all duration-500"
          style={{ left: `${positionPercentage}%` }}>
          {/* Tooltip */}
          <span className="absolute bottom-5 left-1/2 -translate-x-1/2 px-2 py-1 text-[10px] font-semibold text-white bg-indigo-800 rounded shadow-lg whitespace-nowrap">
            {currentTime.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' })}
          </span>
        </div>
      </div>
    </div>
  );
};
