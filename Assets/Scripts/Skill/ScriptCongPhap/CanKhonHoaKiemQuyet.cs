using UnityEngine;
using LongDH.LongDHModules;
using Skill;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(CanKhonHoaKiemQuyet))]
public class CanKhonHoaKiemQuyet : CongPhap, IMemorySerializable<CanKhonHoaKiemQuyet>
{
    [SerializeField] private int[] buffValues = { 2, 3, 5, 8, 16 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KinhDao, out SecureInt kinhDao);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChiThuy, out SecureInt chiThuy);
        int buff = buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
        if (kinhDao > chiThuy)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, buff);
        }
        else if (chiThuy > kinhDao)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TriLieu, buff);
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Kết thúc hiệp, Nếu số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KinhDao)} lớn hơn số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChiThuy)} thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}. Nếu không thì, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TriLieu)}";
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

    public void Deserialize(IReader reader, ref CanKhonHoaKiemQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}