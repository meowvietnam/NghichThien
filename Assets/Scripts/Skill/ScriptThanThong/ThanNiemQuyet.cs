using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThanNiemQuyet), menuName = "Skill/Thần Thông/" + nameof(ThanNiemQuyet))]
public class ThanNiemQuyet : ThanThong, BaseSkill.ThanThuc, IMemorySerializable<ThanNiemQuyet>
{
    [SerializeField] protected int[] dames = new int[] { 11, 19, 22, 38, 53 };
    [SerializeField] protected int[] buffValues = new int[] { 1, 1, 2, 2, 3 };
    [SerializeField] protected float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        Debug.Log("dame base " + dame);
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        ShowEffect(caster, target);
    }

    protected virtual void ShowEffect(CharacterCombat caster, CharacterCombat target)
    {
        PlayVisualSkill(caster, target);
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public void ExecuteThanThuc(CharacterCombat caster, CharacterCombat target)
    {
        int remainThanThuc = caster.GetThanThuc() - target.GetThanThuc();
        remainThanThuc = Mathf.Clamp(remainThanThuc, 0, int.MaxValue);
        int dame = remainThanThuc * buffValues[GetIndexLevelThanThong(caster)];
        Debug.Log("dame thần thức " + dame);
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill, true);
    }

    public override string GetDesc(int level)
    {
        return
            $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.ThanThuc)} Mỗi nhiều hơn đối thủ một điểm {GetKeyWord(EKeyWord.ThanThuc)} tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThanNiemQuyet value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}