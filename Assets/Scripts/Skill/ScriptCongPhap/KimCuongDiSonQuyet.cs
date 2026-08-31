using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(KimCuongDiSonQuyet))]
public class KimCuongDiSonQuyet : CongPhap, IMemorySerializable<KimCuongDiSonQuyet>
{
    [SerializeField]
    private int[] buffValues = new int[]{19, 23, 27, 31, 35};
    [SerializeField]
    private int require = 70;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (caster.characterStatsTemp.hp >= caster.characterStatsTemp.hpMax * (require / 100f))
        {
            target.AddDamageData(new List<int>{(int)(target.GetDameBuffForTarget() * buffValues[CurrentLevelCongPhapInCombat(caster) - 1] / 100)});
        }
    }

    public override string GetDesc(int level)
    {
        return $"Nếu HP bản thân cao hơn {GetColoredText($"{require}%", "red")} thì thần thông sát thương tăng lên {GetColoredText($"{buffValues[level - 1]}%", "red")}";
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

    public void Deserialize(IReader reader, ref KimCuongDiSonQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}