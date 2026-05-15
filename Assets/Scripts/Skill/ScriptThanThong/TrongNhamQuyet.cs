using LongDH.LongDHModules;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TrongNhamQuyet), menuName = "Skill/Thần Thông/" + nameof(TrongNhamQuyet))]
public class TrongNhamQuyet : ThanThong, IMemorySerializable<TrongNhamQuyet>
{
    [SerializeField]
    private int[] buffValues = new int[]{10, 15, 24, 44, 87};
    [SerializeField]
    private int require = 2;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
    }

    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];
        if (currentTurn == null)
            return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is TrongNhamQuyet) && CheckIsEqualETypeSkill(currentTurn))
        {
            if (currentTurn.khiTho >= require)
            {
                int count = currentTurn.specialThanThongImpactAfterPlaySkill.Count(x => x is TrongNhamQuyet);
                currentTurn.TieuTanLinhKhi(ELinhKhi.Tho, require);
                targetTurn.AddDamageData(new System.Collections.Generic.List<int>{buffValues[GetIndexLevelThanThong(currentTurn)] * count});
                currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
            }
        }
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Lần tiếp theo sử dụng {GetTextLinhKhi(ELinhKhi.Tho)} hệ thần thông gây tổn thương sẽ tiêu hao thêm {GetColoredText($"{require}", "red")} điểm {GetTextLinhKhi(ELinhKhi.Tho)} hệ linh khí tạo thành tổn thương {GetColoredText($"+{buffValues[level - 1]}", "red")} (Có thể cộng dồn)";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TrongNhamQuyet value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}