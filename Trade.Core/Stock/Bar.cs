using System;
using System.Collections.Generic;
using System.Linq;

namespace Trade.Core.Stock
{
    /// <summary>
    /// Бар
    /// </summary>
    public class Bar
    {
        private readonly IList<Quote> _quotes;

        /// <summary>
        /// Время начала бара
        /// </summary>
        public DateTime Time { get; private set; }

        /// <summary>
        /// Цена открытия
        /// </summary>
        public double Open { get; set; }

        /// <summary>
        /// Наибольшая цена
        /// </summary>
        public double High { get; set; }

        /// <summary>
        /// Наименьшая цена
        /// </summary>
        public double Low { get; set; }

        /// <summary>
        /// Цена закрытия
        /// </summary>
        public double Close { get; set; }

        /// <summary>
        /// Объём
        /// </summary>
        public int Volume { get; set; }

        /// <summary>
        /// Средневзвешенная цена
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Инициализирует новый бар
        /// </summary>
        /// <param name="time">Время начала бара</param>
        /// <param name="open">Цена открытия</param>
        /// <param name="high">Наибольшая цена</param>
        /// <param name="low">Наименьшая цена</param>
        /// <param name="close">Цена закрытия</param>
        /// <param name="volume">Объём</param>
        public Bar(DateTime time, double open, double high, double low, double close, int volume)
        {
            Time = time;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
        }

        /// <summary>
        /// Инициализирует новый бар, в котором все цены равны
        /// </summary>
        /// <param name="time">Время начала бара</param>
        /// <param name="price">Цена</param>
        /// <param name="volume">Объём</param>
        public Bar(DateTime time, double price = 0, int volume = 0)
            : this(time, price, price, price, price, volume)
        {
        }

        /// <summary>
        /// Инициализирует новый объект бара из коллекции цен
        /// </summary>
        /// <param name="quotes">Коллекция цен</param>
        /// <param name="barLength">Длина бара</param>
        public Bar(IList<Quote> quotes, TimeSpan barLength)
        {
            _quotes = quotes;
            if (quotes == null)
                throw new ArgumentNullException("quotes");
            if (quotes.Count == 0)
                throw new ArgumentException("quotes must be not empty");
            if (barLength.TotalSeconds <= 0)
                throw new ArgumentOutOfRangeException("barLength", "should be positive");

            var first = quotes[0];
            var last = quotes[quotes.Count - 1];
            Time = GetBarStartTime(first.Time, barLength);
            Open = first.Value;
            Close = last.Value;
            High = quotes.Max(q => q.Value);
            Low = quotes.Min(q => q.Value);
            Volume = quotes.Sum(q => q.Volume);
            Price = quotes.Sum(q => q.Value*q.Volume)/Volume;
        }

        /// <summary>
        /// Инициализирует новый объект бара из коллекции цен
        /// </summary>
        /// <param name="quotes">Коллекция цен</param>
        /// <param name="barLength">Длина бара</param>
        public Bar(IList<StockValue> quotes, TimeSpan barLength)
        {
            if (quotes == null)
                throw new ArgumentNullException("quotes");
            if (quotes.Count == 0)
                throw new ArgumentException("quotes must be not empty");
            if (barLength.TotalSeconds <= 0)
                throw new ArgumentOutOfRangeException("barLength", "should be positive");

            var first = quotes[0];
            var last = quotes[quotes.Count - 1];
            Time = GetBarStartTime(first.Time, barLength);
            Open = first.Value;
            Close = last.Value;
            High = quotes.Max(q => q.Value);
            Low = quotes.Min(q => q.Value);
            Price = quotes.Sum(q => q.Value)/quotes.Count;
        }

        /// <summary>
        /// Инициализирует новый объект бара из коллекции цен
        /// </summary>
        /// <param name="bars">Коллекция цен</param>
        /// <param name="barLength">Длина бара</param>
        public Bar(IList<Bar> bars, TimeSpan barLength)
        {
            if (bars == null)
                throw new ArgumentNullException("bars");
            if (bars.Count == 0)
                throw new ArgumentException("bars must be not empty");
            if (barLength.TotalSeconds <= 0)
                throw new ArgumentOutOfRangeException("barLength", "should be positive");

            var first = bars[0];
            var last = bars[bars.Count - 1];
            Time = GetBarStartTime(first.Time, barLength);
            Open = first.Open;
            Close = last.Close;
            High = bars.Max(q => q.High);
            Low = bars.Min(q => q.Low);
            Volume = bars.Sum(q => q.Volume);
            Price = bars.Sum(q => q.Price*q.Volume)/Volume;
        }

        /// <summary>
        /// Возвращает время начала бара
        /// </summary>
        /// <param name="time">Точка времени</param>
        /// <param name="barLength">Длина бара</param>
        /// <returns></returns>
        public static DateTime GetBarStartTime(DateTime time, TimeSpan barLength)
        {
            // Получаем число баров с начала дня
            var dayStart = time.Date;
            var bars = (int) ((time - dayStart).Ticks/barLength.Ticks);
            return dayStart + TimeSpan.FromSeconds(barLength.TotalSeconds*bars);
        }

        /// <summary>
        /// ToString()
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return string.Format("{0} {1}/{2}/{3}/{4} x {5}", Time, Open, High, Low, Close, Volume);
        }
    }
}