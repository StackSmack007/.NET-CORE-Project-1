using System;
using System.Globalization;

namespace Junjuria.Common
{
    public class GlobalConstants
    {
        public static readonly int MostPurchasedTotalCount = 10;
        public static readonly int MostCommentedTotalCount = 10;
        public static readonly int MostRatedTotalCount = 10;
        public static readonly int MaximumCountOfAllProductsOnSinglePage=18;
        public static readonly int MaximumCountOfRowEntitiesOnSinglePageForManaging=26;
        public static readonly string MoneySign= "&euro;";
        public static readonly string WeightSign= "kg";
        public static readonly string TimeFormat = "dd/MM/yyyy (H:mm)";

        public static decimal DeliveryFee(double totalPackageWeight)
        {
            return 4m + (decimal)totalPackageWeight * 0.67m;
        }

        public static string TimeFormatAccepted(DateTime time)
        {
          return  time.ToLocalTime().ToString(TimeFormat, CultureInfo.InvariantCulture);
        }


    }
}
