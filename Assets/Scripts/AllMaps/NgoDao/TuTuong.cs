using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TuTuong), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(TuTuong))]
public class TuTuong : NgoDaoCombat, IMemorySerializable<TuTuong>
{
    [SerializeField] private int requireActiveSkil = 5;
    [SerializeField] private ThanThong nguTran;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (caster.specialThanThongImpactAfterPlaySkill.Count(x =>
                x is TranPhapTT || x.GetEKeywordSkillHave().Contains(BaseSkill.EKeyWord.TranPhap)) >= requireActiveSkil)
        {
            Observer.Notify(EventList.Combat.PLAY_SKILL, caster, target, nguTran, new List<LinhKhiTieuTan>());
        }
    }

    public override string GetDes()
    {
        return
            $"Kết thúc hiệp, Số lượng {BaseSkill.GetKeyWord(BaseSkill.EKeyWord.TranPhap)} có ít nhất {BaseSkill.GetColoredText($"{requireActiveSkil}", "red")} thì phóng thích một lần thần thông [Ngự Trận]";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TuTuong value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}