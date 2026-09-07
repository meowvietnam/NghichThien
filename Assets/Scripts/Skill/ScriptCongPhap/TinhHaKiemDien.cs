using LongDH.LongDHModules;
using Newtonsoft.Json;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TinhHaKiemDien))]
public class TinhHaKiemDien : CongPhap, IMemorySerializable<TinhHaKiemDien>
{
    [SerializeField] private int[] buffValues = new int[] { 4, 8, 12, 18, 36 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int buff = buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KinhDao, buff);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChiThuy, buff);
    }

    public override string GetDesc(int level)
    {
        return
            $"Bắt đầu hiệp, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KinhDao)} và {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChiThuy)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TinhHaKiemDien value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}