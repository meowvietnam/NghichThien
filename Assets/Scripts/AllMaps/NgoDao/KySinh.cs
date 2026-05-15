using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(KySinh), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(KySinh))]
public class KySinh : NgoDaoCombat, IMemorySerializable<KySinh>
{
    [SerializeField]
    private int buff = 2;
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrienNhieu, out SecureInt trienNhieu))
        {
            target.AddDamageData(new List<int>{buff});
        }
    }

    public override string GetDes()
    {
        return $"Nếu đối thủ ở trạng thái {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)} thì tổn thương {BaseSkill.GetColoredText($"+{buff}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CHARACTER_BEFORE_TAKE_DAME, ObserverEffect);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KySinh value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}