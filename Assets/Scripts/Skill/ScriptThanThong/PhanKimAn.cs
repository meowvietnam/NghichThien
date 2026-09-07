using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(PhanKimAn), menuName = "Skill/Thần Thông/" + nameof(PhanKimAn))]
public class PhanKimAn : ThanThong, IMemorySerializable<PhanKimAn>
{
    [SerializeField] private int[] deTonThuong;
    [SerializeField] private float delayTimeEffect = 0.5f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.LIEN_KICH, ObserverEffect);
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect2);
        PlayVisualSkill(caster, target);
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return
            $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Mỗi lần phát động hiệu quả Ngũ Hành {GetKeyWord(EKeyWord.LienKich)} thì khiến đối phương nhận phải {GetColoredText($"+{deTonThuong[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DeTonThuong)}";
    }

    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat target = (CharacterCombat)data[1];
        if (currentTurn == null)
            return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is PhanKimAn))
        {
            List<PhanKimAn> listThisSkill =
                currentTurn.specialThanThongImpactAfterPlaySkill.OfType<PhanKimAn>().ToList();
            foreach (var item in listThisSkill)
            {
                target.AddBuffAndDebuffCount(EBuffAndDeBuff.DeTonThuong,
                    deTonThuong[GetIndexLevelThanThong(currentTurn)]);
            }
        }
    }

    private void ObserverEffect2(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref PhanKimAn value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}