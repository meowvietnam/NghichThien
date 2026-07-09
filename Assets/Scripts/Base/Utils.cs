using System;
using System.Globalization;

namespace Base
{
    public static class Utils
    {
        private static readonly Random Random = new();
        private static readonly object RandomLock = new();

        public static string FormatMoney(long money)
        {
            return money.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"));
        }
        public static string GetTime(TimeSpan timeSpan)
        {
            if (timeSpan <= TimeSpan.Zero)
            {
                return "0 giây";
            }
            if (timeSpan.Days > 0) return $"{timeSpan.Days} ngày {timeSpan.Hours} giờ";
            if (timeSpan.Hours > 0) return $"{timeSpan.Hours} giờ {timeSpan.Minutes} phút";
            if (timeSpan.Minutes > 0) return $"{timeSpan.Minutes} phút {timeSpan.Seconds} giây";
            return $"{timeSpan.Seconds} giây";
        }

        public static string GenerateRandomNumber(int length)
        {
            if (length <= 0)
            {
                return string.Empty;
            }

            var chars = new char[length];
            lock (RandomLock)
            {
                for (var i = 0; i < length; i++)
                {
                    chars[i] = (char)('0' + Random.Next(0, 10));
                }
            }

            return new string(chars);
        }
    }
}
