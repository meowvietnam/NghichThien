using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThongHoa), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(ThongHoa))]
public class ThongHoa : NgoDaoCombat, IMemorySerializable<ThongHoa>
{
    [SerializeField] private int buff = 20;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (BaseSkill.CheckIsEqualETypeSkill(caster, eTypeRequire)) // skill caster dùng là hệ hỏa 
        {
            target.AddDamageData(new List<int> { target.GetDameBuffForTarget() * buff * caster.CurrentTurn / 100 });
        }
    }

    public override string GetDes()
    {
        return
            $"Sử dụng {BaseSkill.GetTextLinhKhi(ELinhKhi.Hoa)} hệ thần thông thì tăng lên {BaseSkill.GetColoredText($"X * {buff}%", "red")} điểm sát thương (X là số hiệp hiện tại)";
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

    public void Deserialize(IReader reader, ref ThongHoa value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}