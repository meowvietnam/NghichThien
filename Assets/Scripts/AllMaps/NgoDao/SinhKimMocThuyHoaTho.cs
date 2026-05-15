using LongDH.LongDHModules;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(SinhKimMocThuyHoaTho), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(SinhKimMocThuyHoaTho))]
public class SinhKimMocThuyHoaTho : NgoDaoCombat, IMemorySerializable<SinhKimMocThuyHoaTho>
{
    [SerializeField]
    private ELinhKhi elinhKhi;
    [SerializeField]
    private int countLinhKhi = 2;
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.HapThuLinhKhi(elinhKhi, countLinhKhi);
    }

    public override string GetDes()
    {
        return $"Bắt đầu hiệp, hấp thụ {BaseSkill.GetColoredText($"+{countLinhKhi}", "red")} điểm {BaseSkill.GetTextLinhKhi(elinhKhi)} hệ linh khí";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref SinhKimMocThuyHoaTho value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}