using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThonLinhChu), menuName = "Skill/Thần Thông/" + nameof(ThonLinhChu))]
public class ThonLinhChu : ThanThong, IMemorySerializable<ThonLinhChu>
{
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoTrao, out SecureInt hoTrao);
        caster.Healing(hoTrao);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hoTrao);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Gỡ ra toàn bộ số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} cũng hồi một lượng HP tương đương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThonLinhChu value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}