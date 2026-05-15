using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(DongBang), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(DongBang))]
public class DongBang : NgoDaoCombat, IMemorySerializable<DongBang>
{
    [SerializeField]
    private int requireActiveSkil = 5;
    [SerializeField]
    private int buff = 2;
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        List<LinhKhiTieuTan> linhKhiTieuTans = (List<LinhKhiTieuTan>)data[3];
        int thuyLK = 0;
        foreach (var item in linhKhiTieuTans)
        {
            if (item.eLinhKhi == ELinhKhi.Thuy)
            {
                thuyLK += item.count;
            }
        }

        if (caster.IsSkillTichLuyCanUse(this, thuyLK, requireActiveSkil))
        {
            target.AddBuffAndDebuffCount(EBuffAndDeBuff.SuongGia, buff);
        }
    }

    public override string GetDes()
    {
        return $"Mỗi tích lũy sử dụng {BaseSkill.GetColoredText($"{requireActiveSkil}", "red")} điểm {BaseSkill.GetTextLinhKhi(ELinhKhi.Thuy)} hệ linh khí, đối thủ nhận phải {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SuongGia)}";
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

    public void Deserialize(IReader reader, ref DongBang value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}