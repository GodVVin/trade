using System;
using System.Collections.Generic;
using System.Linq;
using Trade.Core.Stock;

namespace Trade.Core.Orders
{
    public static class OperationExtensions
    {
        public static Operation Invert(this Operation operation)
        {
            return operation == Operation.Buy ? Operation.Sell : Operation.Buy;
        }
    }

    public enum OrderType
    {
        /// <summary>
        /// Поставить в очередь
        /// </summary>
        Simple,
        /// <summary>
        /// Market order
        /// </summary>
        Market,
        /// <summary>
        /// Limit order
        /// </summary>
        Limit,
        StopOrder
    }

    public enum OrderStatus
    {
        NotReady,
        /// <summary>
        /// Принята торговой системой
        /// </summary>
        Accepted,
        /// <summary>
        /// Активная
        /// </summary>
        Active,
        /// <summary>
        /// Исполнена
        /// </summary>
        Completed,
        /// <summary>
        /// Отменена
        /// </summary>
        Canceled,
        /// <summary>
        /// Отклонена
        /// </summary>
        Rejected,
        Unknown
    }

    public enum CancelResult
    {
        Canceled,
        NotFound,
        Error
    }

    public class OrderState
    {
        public DateTime Timestamp;
        public OrderStatus Status;
        public int Balance;
        
        public override string ToString()
        {
            return "(" + Timestamp.ToLongTimeString() + ":" + Status + ")";
        }
    }

    public sealed class Order
    {
        public OrderType Type { get; set; }
        public Operation Operation { get; set; }
        public Security Security { get; set; }
        public double Price { get; set; }
        public double AlertPrice { get; set; }
        public int Quantity { get; set; }
        public int CompletedCount {get; set; }
        public int Balance { get; private set; }
        public long OrderNo { get; set; }
        public bool IsError { get; set; }
        public OrderStatus Status { get; private set; }
        public List<OrderState> States { get; private set; }
        public List<Stock.Trade> Trades { get; private set; }
        
        public TimeSpan LimitCancelTimeout { get; set; }
        public bool Canceling { get; set; }
        public DateTime OpenTime { get { return States[0].Timestamp; } }
        public double SpreadPriceDelta { get; set; }

        public Order()
        {
            States = new List<OrderState>();
            Trades = new List<Stock.Trade>();
            SpreadPriceDelta = 0;
            CompletedCount = 0;
        }

        public void SetState(OrderState state)
        {
            lock (_locker)
            {
                if (state.Status != OrderStatus.Completed) {
                    Status = state.Status;
                }
                States.Add(state);
            }
        }
        
        public void AddTrade(Stock.Trade trade)
        {
            if (trade == null)
                throw new ArgumentNullException("trade");
            lock (_locker)
            {
                CompletedCount += trade.Quantity;
                Balance = Quantity - CompletedCount;
                
                if (CompletedCount == Quantity)
                    Status = OrderStatus.Completed;
            
                Trades.Add(trade);
            }
        }

        /// <summary>
        /// Средняя цена по всем сделкам
        /// </summary>
        public double AveragePrice
        {
            get { return SumValue / CompletedCount; }
        }

        /// <summary>
        /// Суммарный объём всех сделок
        /// </summary>
        public double SumValue
        {
            get { return Trades.Sum(t => t.Price*t.Quantity); }
        }

        public string StatesStr()
        {
            var states = from state in States
                         select state.ToString();
            return string.Join(" => ", states.ToArray());
        }

        public override string ToString()
        {
            return string.Format("[{0} ({1}) {2} {3} x {4}, completed {5}, balance {6}]",
                                 OrderNo, Type, Operation, Quantity, Price, CompletedCount, Balance);
        }
        
        private readonly object _locker = new object();
    }
}
