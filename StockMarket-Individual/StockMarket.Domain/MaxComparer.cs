namespace StockMarket.Domain
{
    internal class MaxComparer : BaseComparer
    {
        protected override int SpecificCompare(Order? x, Order? y)
        {
            if (x.Price < y.Price) return 1;
            else if (x.Price > y.Price) return -1;
            return 0;
        }
    }
}