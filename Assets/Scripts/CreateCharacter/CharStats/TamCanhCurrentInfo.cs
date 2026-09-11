using System;

namespace CreateCharacter.CharStats
{
    public class TamCanhCurrentInfo : CurrentInfo
    {
        public override string GetValue(object value)
        {
            var val = Convert.ToInt32(value);
            var tamCanh = GameManager.tamCanh;
            if (val < 0)
            {
                return $"{tamCanh[0]} ({value})";
            }
            else if (val <= 49)
            {
                return $"{tamCanh[1]} ({value})";
            }
            else if (val <= 199)
            {
                return $"{tamCanh[2]} ({value})";
            }
            else if (val <= 599)
            {
                return $"{tamCanh[3]} ({value})";
            }
            else if (val <= 999)
            {
                return $"{tamCanh[4]} ({value})";
            }
            else if (val <= 1999)
            {
                return $"{tamCanh[5]} ({value})";
            }
            else
            {
                return $"{tamCanh[6]} ({value})";
            }
        }
    }
}