using System;
using Trade.Core.Stock;

namespace Trade.Core
{
    public sealed class SecuritiesUpdatedEventArgs : EventArgs
    {
        public readonly Security Security;
        public DateTime RowUpdatedTime;

        public SecuritiesUpdatedEventArgs(DateTime updatedTime, Security security)
        {
            RowUpdatedTime = updatedTime;
            Security = security;
        }

        public override string ToString()
        {
            return string.Format("[{0} {1}]", RowUpdatedTime, Security);
        }
    }
}