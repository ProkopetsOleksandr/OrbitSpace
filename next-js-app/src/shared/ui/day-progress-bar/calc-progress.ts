const MINUTES_IN_DAY = 24 * 60;

/**
 * Вспомогательная функция для вычисления позиции точки.
 *
 * @param currentTime - Текущее время в минутах от начала суток (0-1439).
 * @param startMinutes - Начало отрезка в минутах от начала суток.
 * @param endMinutes - Конец отрезка в минутах от начала суток.
 * @returns Процент, на котором находится точка (от 0 до 100).
 */
export function calculatePositionPercentage(currentTime: number, startMinutes: number, endMinutes: number): number {
  let segmentLength: number;
  let timeElapsed: number;

  if (endMinutes < startMinutes) {
    // Случай перехода через полночь (напр. 22:00 - 06:00)
    segmentLength = MINUTES_IN_DAY - startMinutes + endMinutes;

    if (currentTime >= startMinutes) {
      timeElapsed = currentTime - startMinutes;
    } else if (currentTime <= endMinutes) {
      timeElapsed = MINUTES_IN_DAY - startMinutes + currentTime;
    } else {
      // Если время "между" концом и началом (напр. днем при ночном графике)
      // Решаем, к какому краю ближе точка
      return currentTime > endMinutes && currentTime < startMinutes ? 0 : 100;
    }
  } else {
    // Стандартный случай (напр. 08:00 - 22:00)
    segmentLength = endMinutes - startMinutes;
    timeElapsed = currentTime - startMinutes;

    if (currentTime < startMinutes) return 0;
    if (currentTime > endMinutes) return 100;
  }

  if (segmentLength <= 0) return 0;

  const percentage = (timeElapsed / segmentLength) * 100;

  return Math.min(100, Math.max(0, percentage));
}
