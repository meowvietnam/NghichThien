using System;
using System.Collections.Generic;
using System.Globalization;

namespace Base
{
    public static class Utils
    {
        private static readonly Random Random = new();
        private static readonly object RandomLock = new();

        public static string FormatCount(long money)
        {
            return money.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"));
        }

        private static readonly string[] ShortUnits = { "", "K", "M", "B", "T" };

        /// <summary>
        /// Rút gọn số lớn, giữ 3 chữ số có nghĩa và làm tròn xuống: 999 → 999, 827_400 → 827K,
        /// 1_234_567 → 1.23M, 12_500_000 → 12.5M, 2_000_000_000 → 2B.
        /// </summary>
        public static string FormatShort(long value)
        {
            if (value < 0) return "-" + FormatShort(value == long.MinValue ? long.MaxValue : -value);
            if (value < 1000) return value.ToString(CultureInfo.InvariantCulture);
            var unit = 0;
            double v = value;
            while (v >= 1000 && unit < ShortUnits.Length - 1)
            {
                v /= 1000;
                unit++;
            }

            var decimals = v >= 100 ? 0 : v >= 10 ? 1 : 2;
            var factor = Math.Pow(10, decimals);
            v = Math.Floor(v * factor + 1e-9) / factor;
            var text = v.ToString("F" + decimals, CultureInfo.InvariantCulture);
            if (text.Contains(".")) text = text.TrimEnd('0').TrimEnd('.');
            return text + ShortUnits[unit];
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

        public static List<T[]> SplitArray<T>(T[] array, int parts)
        {
            if (parts <= 0)
                throw new ArgumentException("parts must be greater than 0");

            var result = new List<T[]>();

            int baseSize = array.Length / parts;
            int remainder = array.Length % parts;

            int index = 0;

            for (int i = 0; i < parts; i++)
            {
                int size = baseSize + (i < remainder ? 1 : 0);

                var chunk = new T[size];
                Array.Copy(array, index, chunk, 0, size);

                result.Add(chunk);
                index += size;
            }

            return result;
        }

        public static string FormatTimeOnly(TimeSpan time)
        {
            return $"{time.Hours:D2}:{time.Minutes:D2}";
        }
    }
}