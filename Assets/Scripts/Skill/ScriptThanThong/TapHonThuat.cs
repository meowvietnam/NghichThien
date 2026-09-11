using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TapHonThuat), menuName = "Skill/Thần Thông/" + nameof(TapHonThuat))]
public class TapHonThuat : ThanThong, IMemorySerializable<TapHonThuat>
{
    [SerializeField] private int[] dames = new int[] { 12, 16, 26, 48, 96 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.AnNac, out SecureInt anNac);
        if (anNac > 0)
        {
            dame *= 2;
        }

        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
    }

    public override string GetDesc(int level)
    {
        return
            $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Nếu bạn có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnNac)} thì tổn thương gấp đôi";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TapHonThuat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}