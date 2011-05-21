using System;

namespace Trade.Core.Helpers
{
    public static class DateTimeEx
    {
        /// <summary>
        /// Возвращает время начала бара, в который попадает заданная точка времени
        /// </summary>
        /// <param name="date"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public static DateTime GetPeriodStart(this DateTime date, TimeSpan period)
        {
            // Период в секундах
            var periodSeconds = period.TotalSeconds;
            if (periodSeconds == 0)
                return date;
            // Получаем число баров с начала дня
            DateTime dayStart = date.Date;
            // Целое число периодов с начала дня
            int periods = (int)((date - dayStart).TotalSeconds / periodSeconds);
            return dayStart + TimeSpan.FromSeconds(periods * periodSeconds);
        }

        /// <summary>
        /// Возвращает время начала бара, в который попадает заданная точка времени
        /// </summary>
        /// <param name="date"></param>
        /// <param name="periodSeconds"></param>
        /// <returns></returns>
        public static DateTime GetPeriodStart(this DateTime date, int periodSeconds)
        {
            if (periodSeconds == 0)
                return date;
            // Получаем число баров с начала дня
            DateTime dayStart = date.Date;
            // Целое число периодов с начала дня
            int periods = (int)((date - dayStart).TotalSeconds / periodSeconds);
            return dayStart + TimeSpan.FromSeconds(periods * periodSeconds);
        }
    }
}
