using System;

namespace Trade.Core.Stock
{
    public interface IStockData<T>
    {
        int Count { get; }
        T this[int i] { get; }
        T Last { get; }
        string Name { get; }
        bool Updatable { get; }
        event EventHandler<EventArgs> UpdateEvent;
    }
}