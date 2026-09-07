using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(VanKiemQuyet), menuName = "Skill/Thần Thông/" + nameof(VanKiemQuyet))]
public class VanKiemQuyet : ThanThong, IMemorySerializable<VanKiemQuyet>
{
    [SerializeField]
    private int[] dames = new int[]{6, 8, 14, 25, 50};
    [SerializeField]
    private int[] buffValues = new int[]{3, 3, 4, 4, 5};
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out SecureInt kiemKhi);
        int dame = dames[GetIndexLevelThanThong(caster)] + kiemKhi * buffValues[GetIndexLevelThanThong(caster)];
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, kiemKhi);
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dame}, eTypeSkill);
        PlayVisualSkill(caster, target);
    }
    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.KiemY)} Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Tiêu hao tất cả {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}, mỗi một tầng tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref VanKiemQuyet value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}