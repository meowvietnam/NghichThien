using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(NguTangLuyenTranCong))]
public class NguTangLuyenTranCong : CongPhap, IMemorySerializable<NguTangLuyenTranCong>
{
    [SerializeField] private int[] buffValues = new int[] { 1, 2, 3, 4, 6 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (CheckIsEqualETypeSkill(caster))
        {
            int sum = caster.thanThongCurrentUse.linhKhiTieuTan.Sum(x => x.count);
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Tran,
                sum * buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Khi sử dụng {GetTextLinhKhi(ELinhKhi.Tho)} hệ thần thông và phát động ngũ hành {GetKeyWord(EKeyWord.LienKich)} thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]} * X", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)} (X là tổng số linh khí của thân thông)";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.LIEN_KICH, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref NguTangLuyenTranCong value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}