using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TichDietChi), menuName = "Skill/Thần Thông/" + nameof(TichDietChi))]
public class TichDietChi : ThanThong, IMemorySerializable<TichDietChi>
{
    [SerializeField] private int[] dames = new int[] { 50, 100, 200, 400, 800 };
    [SerializeField] private int[] buffValues = new int[] { 30, 60, 120, 240, 480 };
    [SerializeField] private float delayDelete = 1.5f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        if (caster.CurrentLinhKhi <= 0)
        {
            dame += buffValues[GetIndexLevelThanThong(caster)];
        }

        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        PlayVisualSkill(caster, target);
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, delayDelete);
    }

    public override string GetDesc(int level)
    {
        return
            $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Nếu sử dụng thần thông này mà bản thân không còn thừa linh khí nào thì tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TichDietChi value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}