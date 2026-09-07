using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(QuyTran), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(QuyTran))]
public class QuyTran : NgoDaoCombat, IMemorySerializable<QuyTran>
{
    [SerializeField] private ThanThong hoanTran;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Cat, out SecureInt cat))
        {
            Observer.Notify(EventList.Combat.PLAY_SKILL, caster, target, hoanTran, new List<LinhKhiTieuTan>());
        }
    }

    public override string GetDes()
    {
        return
            $"Kết thúc hiệp, nếu đối thủ có {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Cat)} thì phát động một lần [Hoàn Trần]";
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

    public void Deserialize(IReader reader, ref QuyTran value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}