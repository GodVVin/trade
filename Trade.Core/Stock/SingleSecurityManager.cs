using System;
using System.Collections.Generic;
using Trade.Core.Threading;

namespace Trade.Core.Stock
{
    public class NewQuoteEventArgs : EventArgs
    {
        public BidAskQuote Quote { get; private set; }

        public NewQuoteEventArgs(BidAskQuote quote)
        {
            Quote = quote;
        }
    }

    public sealed class SingleSecurityManager
    {
        public event EventHandler<NewQuoteEventArgs> NewQuote;

        public Security Security { get; private set; }
        public readonly StockData<BidAskQuote> Quotes = new StockData<BidAskQuote>();

        public SingleSecurityManager(ISecurities securities, string secCode)
        {
            SecCode = secCode;
            Security = securities[secCode];
            _worker = new Worker<NewQuoteEventArgs>("[" + SecCode + " worker]");
            _worker.NewEvent += WorkerNewEvent;
            _worker.Start();
            securities.UpdatedEvent += SecuritiesUpdated;
        }

        public Bars GetBars(int barLength)
        {
            if (barLength <= 0) throw new ArgumentNullException();
            if (!_barsCache.ContainsKey(barLength))
                _barsCache.Add(barLength, new Bars(_stockData, barLength));
            return _barsCache[barLength];
        }

        private string SecCode { get; set; }
        private readonly StockData<StockValue> _stockData = new StockData<StockValue>();
        private readonly Dictionary<int, Bars> _barsCache = new Dictionary<int, Bars>();

        private readonly Worker<NewQuoteEventArgs> _worker;

        private void SecuritiesUpdated(object sender, SecuritiesUpdatedEventArgs e)
        {
            if (e.Security.SecCode == SecCode)
            {
                Security = e.Security;

                var value = new StockValue {Time = Security.Updated, Value = Security.LastDeal};
                _stockData.AddValue(value);

                var quote = new BidAskQuote
                            {
                                Security = Security,
                                Value = Security.LastDeal,
                                Ask = Security.Ask,
                                Bid = Security.Bid,
                                Time = Security.Updated
                            };
                Quotes.AddValue(quote);
                _worker.AddEvent(new NewQuoteEventArgs(quote));
            }
        }

        private void WorkerNewEvent(object sender, NewQuoteEventArgs e)
        {
            if (NewQuote != null)
                NewQuote(null, e);
        }
    }
}