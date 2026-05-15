using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(KhaiSonQuyet), menuName = "Skill/Thần Thông/" + nameof(KhaiSonQuyet))]
public class KhaiSonQuyet : ThanThong, BaseSkill.TuongDong, IMemorySerializable<KhaiSonQuyet>
{
    [SerializeField]
    private int[] dames = new int[]{7, 9, 11, 17, 45};
    [SerializeField]
    private int[] dameBuffs = new int[]{4, 6, 8, 10, 12};
    [SerializeField]
    private int[] dameBuffPerLinhKhi = new int[]{1, 2, 3, 4, 5};
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)] + (dameBuffPerLinhKhi[GetIndexLevelThanThong(caster)] * caster.CurrentLinhKhi);
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dame}, eTypeSkill);
        ShowSkillEffectWorld(target, delayTimeEffect);
    }

    public void ExecuteTuongDong(CharacterCombat caster, CharacterCombat target)
    {
        int dame = dameBuffs[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dame}, eTypeSkill, true);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Mỗi linh khí còn thừa thì tăng lên {GetColoredText($"+{dameBuffPerLinhKhi[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.TuongDong)} thì tăng lên {GetColoredText($"+{dameBuffs[level - 1]}", "red")} điểm sát thương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KhaiSonQuyet value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}