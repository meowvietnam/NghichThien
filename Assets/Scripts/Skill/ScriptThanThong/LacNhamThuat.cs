using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(LacNhamThuat), menuName = "Skill/Thần Thông/" + nameof(LacNhamThuat))]
public class LacNhamThuat : ThanThong, IMemorySerializable<LacNhamThuat>
{
    [SerializeField]
    private int[] dames = new int[]{12, 16, 26, 45, 90};
    [SerializeField]
    private int[] dameBuffPerDanLuc = new int[]{3, 6, 12, 24, 48};
    [SerializeField]
    protected float delayTimeEffect = 0.85f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DanLuc, out SecureInt danLuc);
        int dame = dames[GetIndexLevelThanThong(caster)] + (dameBuffPerDanLuc[GetIndexLevelThanThong(caster)] * danLuc);
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dame}, eTypeSkill);
        ShowSkillEffectWorld(target, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Đối thủ mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DanLuc)} thì tăng lên {GetColoredText($"+{dameBuffPerDanLuc[level - 1]}", "red")} điểm sát thương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref LacNhamThuat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}