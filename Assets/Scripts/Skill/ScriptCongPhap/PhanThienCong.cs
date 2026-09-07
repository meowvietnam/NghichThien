using LongDH.LongDHModules;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(PhanThienCong))]
public class PhanThienCong : CongPhap, IMemorySerializable<PhanThienCong>
{
    [SerializeField] private int[] buffValues = new int[] { 1, 1, 2, 2, 3 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThieuDot, out SecureInt myThieuDot);
        if (CheckIsEqualETypeSkill(caster))
        {
            int dame = buffValues[CurrentLevelCongPhapInCombat(caster) - 1] * myThieuDot;
            target.AddDamageData(new System.Collections.Generic.List<int> { dame });
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Bản thân mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)} thì {GetTextLinhKhi(ELinhKhi.Hoa)} hệ thần thông sát thương {GetColoredText($"+{buffValues[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref PhanThienCong value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}