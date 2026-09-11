using System;
using System.Collections.Generic;
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
        public Dictionary<BaseSkill.ETypeSkill, int> GachaThanThongDict { get; set; }
        public Dictionary<BaseSkill.ETypeSkill, int> GachaCongPhapDict { get; set; }
        public int Threshold { get; set; }

        public void Set(ETypeGacha type, BaseSkill.ETypeSkill skill, int value)
        {
            var oldValue = Get(type, skill);
            if (oldValue == value)
            {
                return;
            }

            switch (type)
            {
                case ETypeGacha.ThanThong:
                    GachaThanThongDict[skill] = value;
                    break;
                case ETypeGacha.CongPhap:
                    GachaCongPhapDict[skill] = value;
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

        public int Get(ETypeGacha type, BaseSkill.ETypeSkill skill)
        {
            switch (type)
            {
                case ETypeGacha.ThanThong:
                    if (GachaThanThongDict.TryGetValue(skill, out var ttValue))
                    {
                        return ttValue;
                    }

                    return 0;
                case ETypeGacha.CongPhap:
                    if (GachaCongPhapDict.TryGetValue(skill, out var cpValue))
                    {
                        return cpValue;
                    }

                    return 0;
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