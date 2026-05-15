using Skill;
using UnityEngine;
using static BaseSkill;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TuDiem), menuName = "Skill/Thần Thông/" + nameof(TuDiem))]
public class TuDiem : ThanThong, IMemorySerializable<TuDiem>
{
    [SerializeField]
    private int hapThu = 2;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.HapThuLinhKhi(ELinhKhi.Hoa, hapThu);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoaDiem, out SecureInt hoaDiem);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Diem, hoaDiem);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.HoaDiem, hoaDiem);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Hấp thụ {GetColoredText($"+{hapThu}", "red")} điểm {GetTextLinhKhi(ELinhKhi.Hoa)} hệ linh khí. Đồng thời, chuyển số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoaDiem)} sang cho {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Diem)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TuDiem value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}