using System.Globalization;

namespace Base
{
    public static class Utils
    {
        public static string FormatMoney(long money)
        {
            return money.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"));
        }
    }
}