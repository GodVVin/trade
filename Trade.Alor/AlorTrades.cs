using System;
using Trade.Core;
using Trade.Core.Terminals;

namespace Trade.Alor
{
    public class AlorTrades : AlorTable, ITrades
    {
        public AlorTrades(int slotId)
            : base(slotId, "TRADES", false)
        {
        }

        #region Члены ITrades

        public event EventHandler<TradesUpdatedEventArgs> Updated;

        public void Open()
        {
            Connect();
        }

        public void Close()
        {
            Disconnect();
        }

        #endregion

        protected override void OnRowUpdated(RowUpdateType updateType, int rowId)
        {
            lock (_locker)
            {
                if (Updated != null)
                {
                    var tradeNo = GetTradeNo(rowId);
                    var orderNo = GetOrderNo(rowId);
                    var quantity = GetQuantity(rowId);
                    var price = GetPrice(rowId);
                    var value = GetValue(rowId);
                    var e = new TradesUpdatedEventArgs(DateTime.Now, tradeNo, orderNo, quantity, price, value);
                    Updated(this, e);
                }
            }
        }

        private readonly object _locker = new object();

        #region Methods for get values

        private long GetTradeNo(int rowId)
        {
            return (long)GetValue(rowId, "TradeNo");
        }

        private long GetOrderNo(int rowId)
        {
            return (long)GetValue(rowId, "OrderNo");
        }

        private int GetQuantity(int rowId)
        {
            return (int)GetValue(rowId, "Quantity");
        }

        private double GetPrice(int rowId)
        {
            return (double)GetValue(rowId, "Price");
        }

        private double GetValue(int rowId)
        {
            return (double)GetValue(rowId, "Value");
        }

        #endregion
    }
}
