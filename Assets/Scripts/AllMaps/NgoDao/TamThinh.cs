using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TamThinh), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(TamThinh))]
public class TamThinh : NgoDaoCombat, IMemorySerializable<TamThinh>
{
    [SerializeField]
    private int buff = 1;
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        List<LinhKhiTieuTan> linhKhiTieuTans = (List<LinhKhiTieuTan>)data[3];
        if (BaseSkill.CheckIsEqualETypeSkill(caster, eTypeRequire) && CombatManager.GetETuongQuan(linhKhiTieuTans) == ETuongQuan.TuongSinh)
        {
            caster.characterStatsTemp.thanThuc += buff;
        }
    }

    public override string GetDes()
    {
        return $"Sử dụng thần thức hệ thần thông tiêu hao {BaseSkill.GetKeyWord(BaseSkill.EKeyWord.TuongSinh)} linh khí thì Thần thức {BaseSkill.GetColoredText($"+{buff}", "red")}";
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

    public void Deserialize(IReader reader, ref TamThinh value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}