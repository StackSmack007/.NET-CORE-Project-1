namespace Junjuria.Common
{
    public class GlobalConstants
    {
        public static readonly int MostPurchasedTotalCount = 10;
        public static readonly int MostCommentedTotalCount = 10;
        public static readonly int MostRatedTotalCount = 10;
        public static readonly int MaximumCountOfAllProductsOnSinglePage=18;
        public static readonly int MaximumCountOfAllProductsOnSinglePageForManaging=30;
        public static readonly string MoneySign= "&euro;";
        public static readonly string WeightSign= "kg";

        public static decimal DeliveryFee(double totalPackageWeight)
        {
            return 4m + (decimal)totalPackageWeight * 0.67m;
        }

    }
}
