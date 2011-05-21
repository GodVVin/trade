using System;
using System.Runtime.Serialization;

namespace Trade.Core
{
    [Serializable]
    public class TradeException : Exception
    {
        public TradeException()
            : base("TradeException")
        {
        }

        public TradeException(string message)
            : base(message)
        {
        }

        public TradeException(string message, Exception exception)
            : base(message, exception)
        {
        }

        protected TradeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
