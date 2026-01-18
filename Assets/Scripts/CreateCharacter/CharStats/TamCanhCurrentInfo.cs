using System;

namespace CreateCharacter.CharStats
{
    public class TamCanhCurrentInfo : CurrentInfo
    {
        public override string GetValue(object value)
        {
            var val = Convert.ToInt32(value);
            if (val >= 0)
            {
                return $"Thần tĩnh khí an ({value})";
            }
            else
            {
                return $"Tâm phiền ý loạn ({value})";
            }
        }
    }
}