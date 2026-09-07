using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThaiCuc), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(ThaiCuc))]
public class ThaiCuc : NgoDaoCombat, IMemorySerializable<ThaiCuc>
{
    [SerializeField] private ThanThong khuTran;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ThanThong thanThong = (ThanThong)data[2];
        if ((thanThong is TranPhapTT || thanThong.GetEKeywordSkillHave().Contains(BaseSkill.EKeyWord.TranPhap)))
        {
            Observer.Notify(EventList.Combat.PLAY_SKILL, caster, target, khuTran, new List<LinhKhiTieuTan>());
        }
    }

    public override string GetDes()
    {
        return
            $"Mỗi lần sử dụng {BaseSkill.GetKeyWord(BaseSkill.EKeyWord.TranPhap)} thì phóng thích một lần thần thông [Khu Trận]";
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

    public void Deserialize(IReader reader, ref ThaiCuc value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}