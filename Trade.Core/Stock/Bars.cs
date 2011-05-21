using System;
using System.Collections.Generic;
using Trade.Core.Helpers;

namespace Trade.Core.Stock
{
    public class Bars : List<Bar>
    {
        private readonly IStockData<StockValue> _base;
        private int _lastUpdatedDataIndex;

        /// <summary>
        /// Длина бара в секундах
        /// </summary>
        private int BarLengthSeconds { get; set; }

        private TimeSpan BarLength { get; set; }

        public event EventHandler<EventArgs> UpdatedEvent;

        public Bars(IStockData<StockValue> data, int barLengthSeconds)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (barLengthSeconds <= 0)
                throw new ArgumentOutOfRangeException("barLengthSeconds");
            _base = data;
            BarLengthSeconds = barLengthSeconds;
            BarLength = TimeSpan.FromSeconds(BarLengthSeconds);
            _lastUpdatedDataIndex = 0;
            if (_base.Updatable) _base.UpdateEvent += BaseUpdated;
            Update();
        }

        private void BaseUpdated(object sender, EventArgs e)
        {
            UpdateAll();
            if (UpdatedEvent != null)
                UpdatedEvent(this, e);
        }

        public void Update()
        {
            UpdateAll();
        }

        /// <summary>
        /// Возвращает время начала бара, в который попадает заданная точка времени
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public DateTime GetBarStartTime(DateTime date)
        {
            return date.GetPeriodStart((int) BarLength.TotalSeconds);
        }


        private Bar GetByTime(DateTime time)
        {
            return Find(v => v.Time == time);
        }

        /// <summary>
        /// Получает бар, в который попадает точка времени
        /// </summary>
        /// <param name="date">Время</param>
        /// <returns>Найденный бар или null</returns>
        private Bar GetBar(DateTime date)
        {
            return GetByTime(GetBarStartTime(date));
        }

        private void UpdateAll()
        {
            //Logger.Instance.Info("Bars.Update _lastUpdatedDataIndex={0}", _lastUpdatedDataIndex);
            if (0 == _base.Count) return;
            for (int i = _lastUpdatedDataIndex; i < _base.Count; ++i)
            {
                var barTime = _base[i].Time;
                var barStartTime = GetBarStartTime(barTime);
                var bar = GetBar(barTime);
                // Если бар создан
                if (bar != null)
                {
                    AddValueToBar(bar, _base[i].Value);
                }
                else
                {
                    if (Count == 0)
                        // Ещё нет баров, создаём новый
                        bar = CreateNewBar(_base[i].Value, barStartTime);
                    else
                    {
                        // Последний бар, его цена закрытия будет использоваться для создания всех промежуточных баров
                        var lastBar = this[Count - 1];
                        var lastBarTime = GetBarStartTime(lastBar.Time);
                        for (DateTime t = lastBarTime + BarLength; t < barStartTime; t += BarLength)
                        {
                            bar = CreateNewBar(lastBar.Close, t);
                            Add(bar);
                        }
                        if (barTime == barStartTime)
                        {
                            // Если новая цена лежит на левой границе бара
                            // Создаём и заполняем все значения бара значением цены
                            bar = CreateNewBar(_base[i].Value, barStartTime);
                        }
                        else
                        {
                            // Иначе создаём новый бар на основе цены прошлого закрытия
                            bar = CreateNewBar(lastBar.Close, barStartTime);
                            AddValueToBar(bar, _base[i].Value);
                        }
                    }
                    Add(bar);
                }
            }
            _lastUpdatedDataIndex = _base.Count - 1;
        }

        /// <summary>
        /// Создаёт новый бар и заполняет новыми значениями
        /// </summary>
        /// <param name="value"></param>
        /// <param name="openTime"></param>
        /// <returns>Новый бар</returns>
        private static Bar CreateNewBar(double value, DateTime openTime)
        {
            return new Bar(openTime, value);
        }

        /// <summary>
        /// Добавляет новое значание в существующий бар
        /// </summary>
        /// <param name="bar">Бар</param>
        /// <param name="value">Новое значение</param>
        private static void AddValueToBar(Bar bar, double value)
        {
            if (bar == null)
                throw new ArgumentNullException("bar");
            if (value > bar.High)
                bar.High = value;
            if (value < bar.Low)
                bar.Low = value;
            bar.Close = value;
        }
    }
}