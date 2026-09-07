using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TamTaiKiemTran), menuName = "Skill/Thần Thông/" + nameof(TamTaiKiemTran))]
public class TamTaiKiemTran : TranPhapTT, IMemorySerializable<TamTaiKiemTran>
{
    [SerializeField] private int[] buffValues = new int[] { 4, 5, 7, 11, 15 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect);
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NhanKiem, out SecureInt NhanKiem);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DiaKiem, out SecureInt DiaKiem);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThienKiem, out SecureInt ThienKiem);
        if (!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong.Id == this.Id)) && NhanKiem > 0 &&
            DiaKiem > 0 && ThienKiem > 0 && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDescTranPhapTT(int level)
    {
        return
            $"Bản thân có {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NhanKiem)}, {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DiaKiem)} và {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThienKiem)} mới có thể sử dụng. Kết thúc hiệp, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TamTaiKiemTran value)
    {
        TranPhapTT baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}