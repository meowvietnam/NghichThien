using System.Globalization;
using Network.Models.Clans;
using UnityEngine;

namespace Clan
{
    internal static class ClanContributionText
    {
        private static readonly CultureInfo NumberCulture = CultureInfo.GetCultureInfo("vi-VN");

        public static string Format(ClanMember member, string separator = "/")
        {
            var total = (member?.TotalContribPoints ?? 0).ToString("N0", NumberCulture);
            var balance = (member?.ContribPoints ?? 0).ToString("N0", NumberCulture);
            return $"Cống hiến: {balance}{separator}{total}";
        }

        public static void Set(UnityEngine.UI.Text text, ClanMember member)
        {
            if (text == null) return;
            text.text = Format(member);
        }
    }
}
