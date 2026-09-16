using LongDH.LongDHModules;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThaiAtHuyenCong))]
public class ThaiAtHuyenCong : CongPhap, IMemorySerializable<ThaiAtHuyenCong>
{
    [SerializeField] private int[] percentDameBuffs;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        SecureInt trienNhieu = 0;
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrienNhieu, out trienNhieu);
        if (trienNhieu > 0 && CheckIsEqualETypeSkill(caster))
        {
            float percent = (float)percentDameBuffs[CurrentLevelCongPhapInCombat(caster) - 1] / 100f;
            long dame = (long)(target.GetDameBuffForTarget() * percent);
            target.AddDamageData(new System.Collections.Generic.List<long> { dame });
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Sử dụng {GetTextTypeSkill()} hệ thần thông gây sát thương nếu đối phương có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)} thì tổn thương {GetColoredText($"+{percentDameBuffs[level - 1]}%", "red")}";
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

    public void Deserialize(IReader reader, ref ThaiAtHuyenCong value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}