using LongDH.LongDHModules;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HuyetDocKinh))]
public class HuyetDocKinh : CongPhap, IMemorySerializable<HuyetDocKinh>
{
    [SerializeField] private int[] healValues;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.Healing(healValues[CurrentLevelCongPhapInCombat(caster) - 1]);
    }

    public override string GetDesc(int level)
    {
        return
            $"Khi đối phương bị kích phát hiệu quả {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)}, bản thân khôi phục {GetColoredText($"+{healValues[level - 1]}", "red")} điểm Hp";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.TRUNG_DOC, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HuyetDocKinh value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}