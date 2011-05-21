using Trade.Core.Stock;

namespace Trade.Core.Helpers
{
    public static class SecurityEx
    {
        public static double OpenPrice(this Security security, Operation operation, bool isLimit = true, int inSpreadLevel = 0)
        {
            double price;
            if (operation == Operation.Buy)
            {
                if (isLimit)
                    price = security.Bid + security.MinStep * inSpreadLevel;
                else
                    price = security.Ask;
            }
            else
            {
                if (isLimit)
                    price = security.Ask - security.MinStep * inSpreadLevel;
                else
                    price = security.Bid;
            }
            return price;
        }
    }
}
