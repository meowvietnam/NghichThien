using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(KhienLinhChu), menuName = "Skill/Thần Thông/" + nameof(KhienLinhChu))]
public class KhienLinhChu : ThanThong, BaseSkill.TuongSinh, IMemorySerializable<KhienLinhChu>
{
    [SerializeField]
    private int decreaseHiep = 1;
    [SerializeField]
    private int[] buffValues = new int[]{2, 3, 4, 6, 9};
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        CombatStatic.AmKinhTTNewTurn(target, caster);
        ShowSkillEffectWorld(target, delayTimeEffect);
    }

    public void ExecuteTuongSinh(CharacterCombat caster, CharacterCombat target)
    {
        caster.Healing(buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDesc(int level)
    {
        return $"Tất cả {GetKeyWord(EKeyWord.AmKinh)} của đối thủ số hiệp {GetColoredText($"-{decreaseHiep}", "red")}. {GetKeyWord(EKeyWord.TuongSinh)} Khôi phục {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm HP";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KhienLinhChu value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}