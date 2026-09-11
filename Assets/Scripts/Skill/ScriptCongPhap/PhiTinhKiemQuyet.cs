using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(PhiTinhKiemQuyet))]
public class PhiTinhKiemQuyet : CongPhap, IMemorySerializable<PhiTinhKiemQuyet>
{
    [SerializeField] private int[] buffValues = new int[] { 1, 2, 3, 4, 5 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (CheckIsEqualETypeSkill(caster))
        {
            var eTuongQuan = CombatManager.GetETuongQuan(caster.thanThongCurrentUse.linhKhiTieuTan);
            if (eTuongQuan == ETuongQuan.TuongSinh)
            {
                caster.AddBuffAndDebuffCount(EBuffAndDeBuff.GiamTonThuong,
                    buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
            }
            else if (eTuongQuan == ETuongQuan.TuongKhac)
            {
                caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi,
                    buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
            }
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Mỗi sử dụng {GetTextTypeSkill(ETypeSkill.Kiem)} hệ thần thông. Nếu {GetKeyWord(EKeyWord.TuongSinh)} thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.GiamTonThuong)}. Nếu {GetKeyWord(EKeyWord.TuongKhac)} thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref PhiTinhKiemQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}