using System;
using MemoryPack;

namespace CreateCharacter
{
    [MemoryPackable]
    public partial class GachaManager
    {   
        public int GachaThanThong { get; set; }
        public int GachaCongPhap { get; set; }
        public int GachaDanDuoc { get; set; }
        public int GachaDuocLieu { get; set; }
        public int GachaVatLieu { get; set; }
        public int Threshold { get; set; }
        
        public void Set(ETypeGacha type, int value)
        {
            switch (type)
            {
                case ETypeGacha.ThanThong:
                    GachaThanThong = value;
                    break;
                case ETypeGacha.CongPhap:
                    GachaCongPhap = value;
                    break;
                case ETypeGacha.DanDuoc:
                    GachaDanDuoc = value;
                    break;
                case ETypeGacha.DuocLieu:
                    GachaDuocLieu = value;
                    break;
                case ETypeGacha.VatLieu:
                    GachaVatLieu = value;
                    break;
            }
        }
        public int Get(ETypeGacha type)
        {
            switch (type)
            {
                case ETypeGacha.ThanThong:
                    return GachaThanThong;
                case ETypeGacha.CongPhap:
                    return GachaCongPhap;
                case ETypeGacha.DanDuoc:
                    return GachaDanDuoc;
                case ETypeGacha.DuocLieu:
                    return GachaDuocLieu;
                case ETypeGacha.VatLieu:
                    return GachaVatLieu;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}