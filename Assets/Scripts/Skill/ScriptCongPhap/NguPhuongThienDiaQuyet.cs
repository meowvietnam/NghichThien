using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(NguPhuongThienDiaQuyet))]
public class NguPhuongThienDiaQuyet : CongPhap, IMemorySerializable<NguPhuongThienDiaQuyet>
{
    [SerializeField]
    private int[] buffValues = new int[]{5, 9, 18, 46, 140};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int count = caster.specialThanThongImpactAfterPlaySkill.Count(x => x is TranPhapTT || x.GetEKeywordSkillHave().Contains(EKeyWord.TranPhap));
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, count * buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, mỗi có một trận pháp thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref NguPhuongThienDiaQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}