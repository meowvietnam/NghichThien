using LongDH.LongDHModules;
using Newtonsoft.Json;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(QuyNguyenChu), menuName = "Skill/Thần Thông/" + nameof(QuyNguyenChu))]
public class QuyNguyenChu : ThanThong, IMemorySerializable<QuyNguyenChu>
{
    [SerializeField] private int hapThu = 5;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect);
    }

    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];
        if (currentTurn == null)
            return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is QuyNguyenChu))
        {
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
            currentTurn.HapThuLinhKhi(hapThu);
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Kết thúc hiệp, hấp thụ {GetColoredText($"{hapThu}", "red")} điểm linh khí";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref QuyNguyenChu value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}