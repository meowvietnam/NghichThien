using UnityEngine;

namespace CreateCharacter.CharStats
{
    public class LinhCanCurrentInfo : CurrentInfo
    {
        private string[] linhCanStrings = new string[]
        {
            "Thiên Linh Căn",
            "Song Linh Căn",
            "Tam Linh Căn",
            "Tứ Linh Căn",
            "Ngũ Linh Căn"
        };

        protected override Color GetColor(object value, object originalValue)
        {
            if (value is LinhCan newValue && originalValue is LinhCan oldValue)
            {
                if (newValue != oldValue)
                {
                    return Color.chartreuse;
                }
            }

            return Color.white;
        }

        public override string GetValue(object value)
        {
            var linhCan = (int)value;
            return linhCanStrings[linhCan];
        }
    }
}