using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThaiAmHuyenDocCong))]
public class ThaiAmHuyenDocCong : CongPhap, IMemorySerializable<ThaiAmHuyenDocCong>
{
    [SerializeField]
    private int[] dameBuffs;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        SecureInt trungDoc = 0;
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrungDoc, out trungDoc);
        if (trungDoc > 0 && CheckIsEqualETypeSkill(caster))
        {
            int dame = dameBuffs[CurrentLevelCongPhapInCombat(caster) - 1];
            target.AddDamageData(new System.Collections.Generic.List<int>{dame});
        }
    }

    public override string GetDesc(int level)
    {
        return $"Sử dụng {GetTextTypeSkill()} hệ thần thông gây sát thương nếu đối phương có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)} thì tổn thương {GetColoredText($"+{dameBuffs[level - 1]}", "red")}";
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

    public void Deserialize(IReader reader, ref ThaiAmHuyenDocCong value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}