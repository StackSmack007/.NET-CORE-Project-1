﻿namespace Junjuria.Common
{
    public class GlobalConstants
    {
        public static readonly int MostPurchasedTotalCount = 10;
        public static readonly int MostCommentedTotalCount = 10;
        public static readonly int MostRatedTotalCount = 10;
        public static readonly int MaximumCountOfAllProductsOnSinglePage=18;

        public static decimal DeliveryFee(double totalPackageWeight)
        {
            return 4m + (decimal)totalPackageWeight * 0.67m;
        }

    }
}
