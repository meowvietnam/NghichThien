using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(HoChu), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(HoChu))]
public class HoChu : NgoDaoCombat, IMemorySerializable<HoChu>
{
    [SerializeField] private int buff = 2;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (BaseSkill.CheckIsEqualETypeSkill(caster, eTypeRequire))
        {
            List<LinhKhiTieuTan> linhKhiTieuTans = (List<LinhKhiTieuTan>)data[3];
            int count = linhKhiTieuTans.Sum(x => x.count);
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buff * count);
        }
    }

    public override string GetDes()
    {
        return
            $"Sử dụng kiếm hệ thần thông thu hoạch được {BaseSkill.GetColoredText($"+{buff} * X", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} (X là số linh khí thần thông này tiêu hao)";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ObserverEffect);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HoChu value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}