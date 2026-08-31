using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(QuyNhat), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(QuyNhat))]
public class QuyNhat : NgoDaoCombat, IMemorySerializable<QuyNhat>
{
    [SerializeField]
    private int buff = 50;
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (caster.CurrentLinhKhi == caster.totalKhi)
        {
            target.AddDamageData(new List<int>{target.GetDameBuffForTarget() * buff / 100});
        }
        else
        {
            caster.Healing(caster.CurrentLinhKhi);
        }
    }

    public override string GetDes()
    {
        return $"Mỗi lần sử dụng thần thông tạo thành tổn thương, nếu số lượng linh khí của ngươi sau đó tương đương linh khí tối đa thì lần này sát thương tăng lên {BaseSkill.GetColoredText($"{buff}%", "red")}. Nếu không, khôi phục điểm HP tương đương số lượng linh khí hiện tại của ngươi";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref QuyNhat value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}