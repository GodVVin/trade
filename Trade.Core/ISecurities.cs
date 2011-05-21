using System;
using Trade.Core.Stock;

namespace Trade.Core
{
    public interface ISecurities
    {
        event EventHandler<SecuritiesUpdatedEventArgs> UpdatedEvent;
        void Open();
        void Close();

        int Count { get; }
        Security this[string secCode] { get; }
        string[] Securities { get; }
    }
}