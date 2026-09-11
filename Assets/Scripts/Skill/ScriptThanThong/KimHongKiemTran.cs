using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(KimHongKiemTran), menuName = "Skill/Thần Thông/" + nameof(KimHongKiemTran))]
public class KimHongKiemTran : TranPhapTT, IMemorySerializable<KimHongKiemTran>
{
    [SerializeField] protected int[] countAttack = new int[] { 2, 3, 4, 5, 6 };
    [SerializeField] protected int[] damePerCountAttack = new int[] { 3, 3, 3, 3, 3 };
    [SerializeField] private GameObject prefabEffect2;
    [SerializeField] private float delayTimeEffect2 = 0.5f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect);
    }

    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        List<int> dames = new List<int>();
        for (int i = 0; i < countAttack[GetIndexLevelThanThong(caster)]; i++)
        {
            dames.Add(damePerCountAttack[GetIndexLevelThanThong(caster)]);
        }

        target.TakeDameByStatic(caster, dames, false, new[] { BaseSkill.ETypeSkill.Kiem });
        PlayVisualSkill(caster, target);
    }

    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, prefabEffect2, delayTimeEffect2);
    }

    public override string GetDescTranPhapTT(int level)
    {
        return
            $"Kết thúc hiệp, tạo thành {GetColoredText($"{countAttack[level - 1]} lần {damePerCountAttack[level - 1]}", "red")} điểm {GetTextTypeSkill(ETypeSkill.Kiem)} hệ sát thương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KimHongKiemTran value)
    {
        TranPhapTT baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}