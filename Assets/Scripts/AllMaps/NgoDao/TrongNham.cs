using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TrongNham), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(TrongNham))]
public class TrongNham : NgoDaoCombat, IMemorySerializable<TrongNham>
{
    [SerializeField]
    private int buff = 25;
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (BaseSkill.CheckIsEqualETypeSkill(caster, eTypeRequire) && target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DanLuc, out SecureInt danLuc))
        {
            target.AddDamageData(new List<int>{(target.allDamages[0] * buff / 100)});
        }
    }

    public override string GetDes()
    {
        return $"Nếu đối thủ ở trạng thái {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DanLuc)} thì sử dụng {BaseSkill.GetTextLinhKhi(ELinhKhi.Tho)} hệ thần thông thì tăng lên {BaseSkill.GetColoredText($"{buff}%", "red")} điểm sát thương";
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

    public void Deserialize(IReader reader, ref TrongNham value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}