'use client';

import { useEffect, useMemo, useState } from 'react';
import { calculatePositionPercentage } from '../lib/dayProgressBarHelpers';

interface DayProgressBarProps {
  startTimeHour: number; // 0-23
  endTimeHour: number; // 0-23
}

export const DayProgressBar = ({ startTimeHour, endTimeHour }: DayProgressBarProps) => {
  // Состояние для хранения текущего времени
  const [now, setNow] = useState(new Date());

  // Эффект для обновления времени каждую минуту
  useEffect(() => {
    // Обновляем сразу, затем устанавливаем интервал
    const intervalId = setInterval(() => {
      setNow(new Date());
    }, 60 * 1000); // Обновляем каждую минуту (60 секунд * 1000 мс)

    // Очистка интервала при размонтировании компонента
    return () => clearInterval(intervalId);
  }, []);

  // Вычисляем константы для расчета
  const startMinutes = useMemo(() => startTimeHour * 60, [startTimeHour]);
  const endMinutes = useMemo(() => endTimeHour * 60, [endTimeHour]);

  // Текущее время в минутах от начала суток
  const currentMinutes = useMemo(() => now.getHours() * 60 + now.getMinutes(), [now]);

  // Вычисляем процентную позицию точки
  const positionPercentage = useMemo(() => {
    return calculatePositionPercentage(currentMinutes, startMinutes, endMinutes);
  }, [currentMinutes, startMinutes, endMinutes]);

  // Форматируем время для отображения
  const formatTime = (hour: number) => {
    const period = hour >= 12 && hour !== 24 ? 'PM' : 'AM';
    const displayHour = hour % 12 === 0 ? (hour === 24 ? 0 : 12) : hour % 12;
    return `${displayHour.toString().padStart(2, '0')}:00 ${period}`;
  };

  // Проверяем, находится ли точка на 0% или 100% для центрирования
  const isStart = positionPercentage <= 0;
  const isEnd = positionPercentage >= 100;

  // Стиль для позиционирования точки
  const dotStyle = {
    // Используем `calc` для центрирования точки:
    // `positionPercentage` минус половина ширины точки (3px/12px)
    left: `calc(${positionPercentage}% - ${isStart || isEnd ? '6px' : '3px'})`
  };

  return (
    <div>
      <h2 className="text-xl font-bold mb-4 text-gray-800">{now.toDateString()}</h2>

      <div className="flex justify-between text-sm font-medium text-gray-600 mb-2">
        <span>{formatTime(startTimeHour)}</span>
        <span>{formatTime(endTimeHour)}</span>
      </div>

      {/* Контейнер для отрезка */}
      <div className="relative h-2 bg-indigo-200 rounded-full">
        {/* Полоса прогресса, если нужно показать, сколько времени уже прошло */}
        {positionPercentage > 0 && positionPercentage < 100 && (
          <div className="absolute top-0 left-0 h-full bg-indigo-600 rounded-full" style={{ width: `${positionPercentage}%` }} />
        )}

        {/* Точка "Сейчас" */}
        <div
          className="absolute top-1/2 -translate-y-1/2 w-3 h-3 bg-indigo-800 rounded-full shadow-md z-10 
                     transition-all duration-300 ease-out"
          style={dotStyle}>
          {/* Всплывающая подсказка с текущим временем */}
          <span
            className="absolute bottom-5 left-1/2 -translate-x-1/2 
                       px-2 py-1 text-xs font-semibold text-white bg-indigo-800 
                       rounded shadow-lg whitespace-nowrap">
            {now.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' })}
          </span>
        </div>
      </div>
    </div>
  );
};
