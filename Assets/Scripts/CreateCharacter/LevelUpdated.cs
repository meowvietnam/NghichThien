using MemoryPack;

namespace CreateCharacter
{
    [MemoryPackable]
    public partial class LevelUpdated
    {
        public int Level { get; set; }
        public int CurrentExp { get; set; }
        public int HpMax { get; set; }
        public int ThanThuc { get; set; }
        public int DonToc { get; set; }
        public int ThoNguyen { get; set; }
        public int NewNgoDaoDiem { get; set; }
        public void ApplyTo(CharacterData charData)
        {
            var progression = charData.Progression;
            progression.Level = Level;
            progression.CurrentExp = CurrentExp;
            var stats = charData.Stats;
            stats.SinhMenh = HpMax;
            stats.ThanThuc = ThanThuc;
            stats.TocDoBay = DonToc;
            stats.ThoNguyen = ThoNguyen;
            progression.NgoDaoDiem = NewNgoDaoDiem;
        }
    }
}