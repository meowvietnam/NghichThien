using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TietLinhThu), menuName = "Skill/Thần Thông/" + nameof(TietLinhThu))]
public class TietLinhThu : ThanThong, IMemorySerializable<TietLinhThu>
{
    [SerializeField]
    private int[] dames = new int[]{3, 4, 6, 10, 20};
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        if (target.CurrentLinhKhi <= 0)
        {
            dame *= 2;
        }

        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dame}, eTypeSkill);
        ShowSkillEffectWorld(target, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Nếu như đối thủ không có linh khí thì tổn thương gấp đôi";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TietLinhThu value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}