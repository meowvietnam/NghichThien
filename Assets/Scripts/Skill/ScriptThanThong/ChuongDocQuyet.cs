using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ChuongDocQuyet), menuName = "Skill/Thần Thông/" + nameof(ChuongDocQuyet))]
public class ChuongDocQuyet : ThanThong, BaseSkill.TuongDong, IMemorySerializable<ChuongDocQuyet>
{
    [SerializeField]
    private int[] dames;
    [SerializeField]
    private int[] dameBuffs;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        SecureInt trungDoc = 0;
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrungDoc, out trungDoc);
        if (trungDoc > 0)
        {
            dame *= 2;
        }

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
        return $"Tạo thành {GetColoredText($"{dames[level - 1]}", "red")} điểm sát thương. Nếu đối phương có {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)} thì nhận tổn thương gấp đôi. Nếu là {GetKeyWord(EKeyWord.TuongDong)} tăng lên {GetColoredText($"{dameBuffs[level - 1]}", "red")} điểm sát thương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ChuongDocQuyet value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}