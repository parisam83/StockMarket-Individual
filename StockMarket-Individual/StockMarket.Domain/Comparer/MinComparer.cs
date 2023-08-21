namespace StockMarket.Domain.Comparer
{
    internal class MinComparer : BaseComparer
    {
        protected override int SpecificCompare(Order? x, Order? y)
        {
            if (x?.Price > y?.Price) return 1;
            else if (x?.Price < y?.Price) return -1;
            return 0;
        }
    }
}