using System;

namespace Trade.Core
{
    public interface ITrades
    {
        event EventHandler<TradesUpdatedEventArgs> Updated;
        void Open();
        void Close();
    }
}
