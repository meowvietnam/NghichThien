using UnityEngine;
using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(HoaHuyetDocChuong), menuName = "Skill/Thần Thông/" + nameof(HoaHuyetDocChuong))]
public class HoaHuyetDocChuong : ThanThong, IMemorySerializable<HoaHuyetDocChuong>
{
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        SecureInt trungDoc = 0;
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrungDoc, out trungDoc);
        HauKeVoLuc(caster);
        CombatStatic.TrungDoc(caster, target);
        caster.Healing(trungDoc);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        SecureInt trungDoc = 0;
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrungDoc, out trungDoc);
        return trungDoc > 0 && base.IsCanUse(caster, target);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Kích phát hiệu ứng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)} của đối thủ. Đồng thời, hồi một lượng Hp bằng với lượng sát thương gây ra";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HoaHuyetDocChuong value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}