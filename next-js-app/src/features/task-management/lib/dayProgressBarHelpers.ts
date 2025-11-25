/**
 * Вспомогательная функция для вычисления позиции точки.
 *
 * @param currentTime - Текущее время в минутах от начала суток (0-1439).
 * @param startMinutes - Начало отрезка в минутах от начала суток.
 * @param endMinutes - Конец отрезка в минутах от начала суток.
 * @returns Процент, на котором находится точка (от 0 до 100).
 */
export function calculatePositionPercentage(currentTime: number, startMinutes: number, endMinutes: number): number {
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
}
