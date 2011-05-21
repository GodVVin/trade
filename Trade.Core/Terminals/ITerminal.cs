using System;
using Trade.Core.Orders;
using Trade.Core.Stock;

namespace Trade.Core.Terminals
{
    public enum TerminalType
    {
        AlorTrade, Oec
    }

    public enum RowUpdateType { Added, Deleted, Updated, Refreshed, Unknown };

    public interface ITerminal
    {
        event EventHandler<SecuritiesUpdatedEventArgs> SecuritiesUpdated;
        event EventHandler<OrdersUpdatedEventArgs> OrdersUpdated;
        event EventHandler<TradesUpdatedEventArgs> TradesUpdated;
        event EventHandler<EventArgs> Error;

        TerminalType Type { get; }

        string Login { get; }

        /// <summary>
        /// Таблица SECURITIES
        /// </summary>
        ISecurities Securities { get; }

        /// <summary>
        /// Таблица ORDERS
        /// </summary>
        IOrders Orders { get; }

        ITrades Trades { get; }

        /// <summary>
        /// 
        /// </summary>
        OrdersManager OrdersManager { get; }

        void Connect();
        void Disconnect();
        void OpenOrders();

        IOrderbook OpenOrderbook(Security security);
    }
}