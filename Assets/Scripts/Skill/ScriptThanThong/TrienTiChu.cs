using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TrienTiChu), menuName = "Skill/Thần Thông/" + nameof(TrienTiChu))]
public class TrienTiChu : ThanThong, IMemorySerializable<TrienTiChu>
{
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        target.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.REMOVE_BUFF, ObserverEffect);
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect2);
        ShowSkillEffectWorld(target, delayTimeEffect);
    }

    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        EBuffAndDeBuff type = (EBuffAndDeBuff)data[1];
        int newValue = (int)data[2];
        int currentValue = (int)data[3];
        if (currentTurn == null)
            return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is TrienTiChu) && newValue <= 0 && type == EBuffAndDeBuff.TrienNhieu)
        {
            currentTurn.AddBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu, currentValue / 2);
        // end turn giữ lại 1 nữa được xử lí ở character combat
        }
    }

    private void ObserverEffect2(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        if (currentTurn == null)
            return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is TrienTiChu))
        {
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
        // end turn giữ lại 1 nữa được xử lí ở character combat
        }
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        SecureInt trienNhieu = 0;
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrienNhieu, out trienNhieu);
        return trienNhieu > 0 && base.IsCanUse(caster, target);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Đối thủ kết thúc hiệp, thì giữ lại một nửa số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TrienTiChu value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}