using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;
using System.Linq;

[CreateAssetMenu(fileName = nameof(LacAnh), menuName = "Skill/Thần Thông/" + nameof(LacAnh))]
public class LacAnh : ThanThong, BaseSkill.LienKich, IMemorySerializable<LacAnh>
{
    [SerializeField] private int[] dames;
    [SerializeField] private int[] percentBuff;
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        PlayVisualSkill(caster, target);
    }

    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectNoTarget(delayTimeEffect);
    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        int countDebuff = 0;
        foreach (var item in target.buffAndDebuffCount)
        {
            if (BaseSkill.debuffWeights.Keys.Contains(item.Key))
            {
                countDebuff++;
            }
        }

        int dame = dames[GetIndexLevelThanThong(caster)];
        float percent = (float)percentBuff[GetIndexLevelThanThong(caster)] / 100f;
        target.CaculateDameByThanThong(caster,
            new System.Collections.Generic.List<int> { (int)(dame * (percent * countDebuff)) }, eTypeSkill, true);
    }

    public override string GetDesc(int level)
    {
        return
            $"Tạo thành {GetColoredText($"{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)}. Nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Thuy)} hệ thần thông, đối phương mỗi có một loại trạng thái tiêu cực thì tổn thương tăng lên {GetColoredText($"{percentBuff[level - 1]}%", "red")}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref LacAnh value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}