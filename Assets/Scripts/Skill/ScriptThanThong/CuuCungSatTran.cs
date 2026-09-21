using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(CuuCungSatTran), menuName = "Skill/Thần Thông/" + nameof(CuuCungSatTran))]
public class CuuCungSatTran : TranPhapTT, IMemorySerializable<CuuCungSatTran>
{
    [SerializeField] private int[] dames = new int[] { 2, 7, 20, 32, 80 };
    [SerializeField] private int[] buffValues = new int[] { 9, 9, 9, 9, 9 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect);
    }

    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        int count = caster.specialThanThongImpactAfterPlaySkill.Count(x => x is CuuCungSatTran);
        int dame = dames[GetIndexLevelThanThong(caster)] + (count - 1) * (buffValues[GetIndexLevelThanThong(caster)]);
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        target.TakeDameForThanThong(caster,true);
        caster.AddThanThongImpactAfterPlaySkill(this);
    }

    public override string GetDescTranPhapTT(int level)
    {
        return
            $"Kết thúc hiệp, Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Mỗi lần như vậy tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref CuuCungSatTran value)
    {
        TranPhapTT baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}