using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TramThienKiem), menuName = "Skill/Thần Thông/" + nameof(TramThienKiem))]
public class TramThienKiem : ThanThong, IMemorySerializable<TramThienKiem>
{
    [SerializeField]
    private int[] dames = new int[]{19, 27, 38, 74, 148};
    [SerializeField]
    private int require = 2;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        int dame = dames[GetIndexLevelThanThong(caster)];
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, require);
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dame}, eTypeSkill);
        PlayVisualSkill(caster, target);
    }
    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out SecureInt kiemKhi);
        if (kiemKhi >= require && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.KiemY)} Tiêu hao {GetColoredText($"{require}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm {GetKeyWord(EKeyWord.ChanThucTonThuong)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TramThienKiem value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}