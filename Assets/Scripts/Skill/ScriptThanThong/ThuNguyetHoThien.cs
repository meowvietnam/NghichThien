using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThuNguyetHoThien), menuName = "Skill/Thần Thông/" + nameof(ThuNguyetHoThien))]
public class ThuNguyetHoThien : ThanThong, IMemorySerializable<ThuNguyetHoThien>
{
    [SerializeField]
    private int[] dames;
    [SerializeField]
    private int[] dameBuffs;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dameFinal = dames[GetIndexLevelThanThong(caster)];
        int dameBuff = dameBuffs[GetIndexLevelThanThong(caster)];
        dameFinal += (dameBuff * caster.CurrentLinhKhi);
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dameFinal}, eTypeSkill);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Mỗi còn thừa một điểm linh khí thì tăng lên {GetColoredText($"+{dameBuffs[level - 1]}", "red")} điểm sát thương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThuNguyetHoThien value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}