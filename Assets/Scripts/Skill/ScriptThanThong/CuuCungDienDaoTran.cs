using LongDH.LongDHModules;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(CuuCungDienDaoTran), menuName = "Skill/Thần Thông/" + nameof(CuuCungDienDaoTran))]
public class CuuCungDienDaoTran : TranPhapTT, IMemorySerializable<CuuCungDienDaoTran>
{
    [SerializeField] private int require = 3;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ObserverEffect);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);
        CheckBoQuaTieuTanLinhKhi(caster, target);
    }

    private void CheckBoQuaTieuTanLinhKhi(CharacterCombat caster, CharacterCombat target)
    {
        if (caster.thanThongUseThisTurn.Count <= require)
        {
            caster.isSkipTieuTanLinhKhi = true;
        }
        else
        {
            caster.isSkipTieuTanLinhKhi = false;
        }
    }

    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        CheckBoQuaTieuTanLinhKhi(caster, target);
    }

    public override string GetDescTranPhapTT(int level)
    {
        return
            $"Ở hiệp này, nếu số lần sử dụng thần thông chưa nhiều hơn {GetColoredText($"{require}", "red")} lần thì nhảy qua giai đoạn tiêu tán linh khí";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref CuuCungDienDaoTran value)
    {
        TranPhapTT baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}