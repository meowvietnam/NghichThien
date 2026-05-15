using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThanHoangAn), menuName = "Skill/Thần Thông/" + nameof(ThanHoangAn))]
public class ThanHoangAn : ThanThong, IMemorySerializable<ThanHoangAn>
{
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        List<LinhKhiTieuTan> linhKhiTieuTan = data[3] as List<LinhKhiTieuTan>;
        bool isTuongDong = CombatManager.IsTuongDong(linhKhiTieuTan);
        if (isTuongDong)
        {
            if (caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SucThe, out SecureInt sucThe))
            {
                caster.AddBuffAndDebuffCount(EBuffAndDeBuff.SucThe, sucThe);
            }
        }
        else
        {
            if (caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoTrao, out SecureInt hoTrao))
            {
                caster.AddBuffAndDebuffCount(EBuffAndDeBuff.SucThe, hoTrao * 2);
                caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hoTrao);
            }
        }
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        SecureInt sucThe = 0;
        SecureInt hoTrao = 0;
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SucThe, out sucThe);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoTrao, out hoTrao);
        if ((sucThe > 0 || hoTrao > 0) && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override string GetDesc(int level)
    {
        return $"Nếu {GetKeyWord(EKeyWord.TuongDong)} thì số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucThe)} tăng gấp đôi. Nếu không thì, chuyển toàn bộ tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} đang có thành {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucThe)}*2";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThanHoangAn value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}