'use client';

import React, { useEffect, useMemo, useState } from 'react';

// Определяем интерфейс для пропсов компонента
interface TimeSegmentDisplayProps {
  // Начало отрезка в часах (0-23)
  startTimeHour: number;
  // Конец отрезка в часах (0-23)
  endTimeHour: number;
}

/**
 * Вспомогательная функция для вычисления позиции точки.
 *
 * @param currentTime - Текущее время в минутах от начала суток (0-1439).
 * @param startMinutes - Начало отрезка в минутах от начала суток.
 * @param endMinutes - Конец отрезка в минутах от начала суток.
 * @returns Процент, на котором находится точка (от 0 до 100).
 */
const calculatePositionPercentage = (currentTime: number, startMinutes: number, endMinutes: number): number => {
  // Если конец раньше начала (например, с 22:00 до 06:00), мы пересчитываем интервал.
  if (endMinutes < startMinutes) {
    // Длина интервала: от начала до 23:59 + от 00:00 до конца.
    const segmentLength = 24 * 60 - startMinutes + endMinutes;

    let timeElapsed: number;
    if (currentTime >= startMinutes) {
      // Время внутри первого дня
      timeElapsed = currentTime - startMinutes;
    } else if (currentTime < endMinutes) {
      // Время внутри второго дня
      timeElapsed = 24 * 60 - startMinutes + currentTime;
    } else {
      // Текущее время вне отрезка
      return -1; // или 0, в зависимости от желаемого отображения
    }

    // Предотвращаем деление на ноль
    if (segmentLength === 0) return 0;

    // Ограничиваем процент от 0 до 100
    const percentage = (timeElapsed / segmentLength) * 100;
    return Math.min(100, Math.max(0, percentage));
  }

  // Стандартный случай, когда начало раньше конца
  const segmentLength = endMinutes - startMinutes;
  const timeElapsed = currentTime - startMinutes;

  // Если время вне отрезка, отображаем его либо в начале, либо в конце, либо скрываем.
  if (currentTime < startMinutes) return 0;
  if (currentTime > endMinutes) return 100;

  // Предотвращаем деление на ноль
  if (segmentLength === 0) return 0;

  // Вычисляем процент и ограничиваем его
  return Math.min(100, Math.max(0, (timeElapsed / segmentLength) * 100));
};

const TimeSegmentDisplay: React.FC<TimeSegmentDisplayProps> = ({ startTimeHour, endTimeHour }) => {
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
    <div className="p-4 md:p-8 bg-gray-50 rounded-xl shadow-lg border border-gray-200 w-full max-w-2xl mx-auto">
      <h2 className="text-xl font-bold mb-4 text-gray-800">Временной отрезок активности</h2>

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

export default TimeSegmentDisplay;
