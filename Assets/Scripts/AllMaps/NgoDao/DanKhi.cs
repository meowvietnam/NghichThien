using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(DanKhi), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(DanKhi))]
public class DanKhi : NgoDaoCombat, IMemorySerializable<DanKhi>
{
    [SerializeField] private int buff = 1;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        CombatStatic.AmKinhTTNewTurn(target, caster);
        int count = target.listAmKinhTT.Select(x => x.ThanThong).Distinct().Count();
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.NoiThuong, count);
    }

    public override string GetDes()
    {
        return
            $"Kết thúc hiệp, Tất cả {BaseSkill.GetKeyWord(BaseSkill.EKeyWord.AmKinh)} của đối thủ số hiệp {BaseSkill.GetColoredText($"-{buff}", "red")}. Mỗi một loại {BaseSkill.GetKeyWord(BaseSkill.EKeyWord.AmKinh)} khác biệt thì đối thủ nhận phải {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NoiThuong)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref DanKhi value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}