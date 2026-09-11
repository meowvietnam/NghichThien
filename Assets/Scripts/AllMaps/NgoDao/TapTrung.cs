using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TapTrung), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(TapTrung))]
public class TapTrung : NgoDaoCombat, IMemorySerializable<TapTrung>
{
    [SerializeField] private ThanThong khuTran;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (caster.specialThanThongImpactAfterPlaySkill.Count(x =>
                x is TranPhapTT || x.GetEKeywordSkillHave().Contains(BaseSkill.EKeyWord.TranPhap)) == 1)
        {
            Observer.Notify(EventList.Combat.PLAY_SKILL, caster, target, khuTran, new List<LinhKhiTieuTan>());
        }
    }

    public override string GetDes()
    {
        return
            $"Kết thúc hiệp, Có một cái {BaseSkill.GetKeyWord(BaseSkill.EKeyWord.TranPhap)} thì phóng thích một lần thần thông [Khu Trận]";
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

    public void Deserialize(IReader reader, ref TapTrung value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}