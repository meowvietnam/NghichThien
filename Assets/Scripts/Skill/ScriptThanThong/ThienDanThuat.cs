using LongDH.LongDHModules;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThienDanThuat), menuName = "Skill/Thần Thông/" + nameof(ThienDanThuat))]
public class ThienDanThuat : ThanThong, IMemorySerializable<ThienDanThuat>
{
    [SerializeField] private int[] buffValues = new int[] { 12, 18, 28, 50, 100 };
    [SerializeField] protected float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
        PlayVisualSkill(caster, target);
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectParticleBase(caster, delayTimeEffect, 5);
    }

    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];
        if (currentTurn == null)
            return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is ThienDanThuat) &&
            CheckIsEqualETypeSkill(currentTurn))
        {
            int count = currentTurn.specialThanThongImpactAfterPlaySkill.Count(x => x is ThienDanThuat);
            targetTurn.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DanLuc, out SecureInt danLuc);
            targetTurn.AddDamageData(new System.Collections.Generic.List<int>
                { buffValues[GetIndexLevelThanThong(currentTurn)] * danLuc * count });
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Lần tiếp theo sử dụng {GetTextLinhKhi(ELinhKhi.Tho)} hệ thần thông gây tổn thương. Đối thủ mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DanLuc)} thì tổn thương {GetColoredText($"+{buffValues[level - 1]}", "red")} (Có thể cộng dồn)";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThienDanThuat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}