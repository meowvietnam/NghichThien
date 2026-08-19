using Base;
using CreateCharacter.CharStats;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class TrungSinhConfig
    {
        public int MoneyVip { get; set; }
        public int ThienPhu { get; set; }
        public int TargetLevel { get; set; }
        public int DaiCanhGioi { get; set; }
        public CharacterStats Stats { get; set; }
        public string GetText()
        {
            var levelText = GameManager.levelWithText[TargetLevel];
            return $"Trùng Sinh tại cấp {levelText}\n" +
                   $"Thiên Phú: {ThienPhu}\n" +
                   $"Tiêu hao: {Utils.FormatCount(MoneyVip)} Tiên ngọc";
        }
        
    }
}