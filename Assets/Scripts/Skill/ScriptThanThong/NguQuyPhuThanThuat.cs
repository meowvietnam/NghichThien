using LongDH.LongDHModules;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(NguQuyPhuThanThuat), menuName = "Skill/Thần Thông/" + nameof(NguQuyPhuThanThuat))]
public class NguQuyPhuThanThuat : ThanThong, IMemorySerializable<NguQuyPhuThanThuat>
{
    [SerializeField]
    private int[] buffs = new int[]{15, 30, 45, 60, 75};
    [SerializeField]
    private int takedDame = 1;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.characterStatsTemp.hpMax += buffs[GetIndexLevelThanThong(caster)];
        caster.Healing(0);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.CHARACTER_BEFORE_TAKE_DAME, ObserverEffect);
        PlayVisualSkill(caster, target);
    }
    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    private void ObserverEffect(params object[] data)
    {
        CharacterCombat targetTurn = (CharacterCombat)data[1];
        if (targetTurn == null)
            return;
        if (targetTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is NguQuyPhuThanThuat))
        {
            targetTurn.AddDamageData(new System.Collections.Generic.List<int>{takedDame});
        }
    }

    public override string GetDesc(int level)
    {
        return $"Tăng tạm thời {GetColoredText($"+{buffs[level - 1]}", "red")} Hp tối đa trong trận chiến này. Nhận tất cả tổn thương {GetColoredText($"+{takedDame}", "red")} {GetColoredText($"Mỗi cuộc chiến chỉ có thể sử dụng một lần", "red")}";
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        if (!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong is NguQuyPhuThanThuat)) && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref NguQuyPhuThanThuat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}