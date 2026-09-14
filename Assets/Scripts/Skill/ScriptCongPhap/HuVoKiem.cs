using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;
using LongDHModules;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HuVoKiem))]
public class HuVoKiem : CPActiveAfterUseSkill, IMemorySerializable<HuVoKiem>
{
  
    private int[] countActiveDame = new int[] { 1, 1, 2, 3, 5 };
    [SerializeField] private float delayTimeEffect = 3f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int indexLevel = CurrentLevelCongPhapInCombat(caster) - 1;
        if (caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out SecureInt kiemKhi))
        {
            List<int>dames = new List<int>();
            for (int i = 0; i < countActiveDame[indexLevel]; i++)
            {
                dames.Add(kiemKhi);
            }
            target.AddDamageData(dames,false);
            PlayVisualSkill(caster,target);
        }

    }
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {   
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectParticleBase(target, delayTimeEffect,5);
    }
    protected override string DescEqual()
    {
        return base.DescEqual() + " gây tổn thương";
    }
    public override string GetDesc(int level)
    {
        return base.GetDesc(level) + $"triệu hồi những kiếm khí từ hư vô bổ trợ {GetColoredText($"{countActiveDame[level - 1]} lần", "red")} X sát thương (Với X là số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} mà ngươi sở hữu)";
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

    public void Deserialize(IReader reader, ref HuVoKiem value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}
