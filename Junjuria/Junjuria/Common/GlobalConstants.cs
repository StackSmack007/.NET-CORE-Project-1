using System;
using System.Globalization;
using System.Text;

namespace Junjuria.Common
{
    public static class GlobalConstants
    {
        private static StringBuilder sb;
        public static readonly int MostPurchasedTotalCount = 10;
        public static readonly int MostCommentedTotalCount = 10;
        public static readonly int MostRatedTotalCount = 10;
        public static readonly int MaximumCountOfAllProductsOnSinglePage = 18;
        public static readonly int MaximumCountOfRowEntitiesOnSinglePageForManaging = 26;
        public static readonly string MoneySign = "&euro;";
        public static readonly string WeightSign = "kg";
        public static readonly string DateHourMinFormatJS ="DD/MM/YYYY (hh:mm)";
        public static readonly string TimeFormat = "dd/MM/yyyy (H:mm)";
        public static readonly string ChatUrlHub = "/chat";
        public static string JunjuriaEmailSenderName = "Junjuria Store";
        public static string JunjuriaEmail = "junjuria_techsupport231@gmail.com";

        static GlobalConstants()
        {
            sb = new StringBuilder();
        }
        public static decimal DeliveryFee(double totalPackageWeight)
        {
            return 4m + (decimal)totalPackageWeight * 0.67m;
        }

        public static string ServerTimeConvert(DateTime time)
        {
            return time.ToLocalTime().ToString(TimeFormat, CultureInfo.InvariantCulture);
        }

        public static string SplitWords(this string word)
        {
            sb.Clear();
            foreach (var symbol in word)
            {
                if (char.IsUpper(symbol)) sb.Append(" ");
                sb.Append(symbol);
            }
            return sb.ToString();
        }
    }
}
