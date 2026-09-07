using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TuPhuongDuLinhDo))]
public class TuPhuongDuLinhDo : CongPhap, IMemorySerializable<TuPhuongDuLinhDo>
{
    [SerializeField] private int[] buffValues = new int[] { 1, 2, 3, 4, 5 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int count = caster.specialThanThongImpactAfterPlaySkill.Count(thanThong =>
            thanThong is TranPhapTT || thanThong.GetEKeywordSkillHave().Contains(EKeyWord.TranPhap));
        if (count > 0)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TriLieu,
                count * buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Kết thúc hiệp, mỗi có một {GetKeyWord(EKeyWord.TranPhap)} thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TriLieu)}";
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

    public void Deserialize(IReader reader, ref TuPhuongDuLinhDo value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}