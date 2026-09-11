using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(LinhLucHoTrao), menuName = "Skill/Thần Thông/" + nameof(LinhLucHoTrao))]
public class LinhLucHoTrao : ThanThong, BaseSkill.TuongSinh, IMemorySerializable<LinhLucHoTrao>
{
    [SerializeField] private int[] buffValues = new int[] { 2, 3, 5, 9, 18 };
    [SerializeField] private int[] buffValues2 = new int[] { 1, 2, 3, 4, 5 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int hoTrao1 = buffValues[GetIndexLevelThanThong(caster)];
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hoTrao1);
    }

    public void ExecuteTuongSinh(CharacterCombat caster, CharacterCombat target)
    {
        int hoTrao2 = buffValues2[GetIndexLevelThanThong(caster)];
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hoTrao2);
    }

    public override string GetDesc(int level)
    {
        return
            $"Thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}. {GetKeyWord(EKeyWord.TuongSinh)} thì thu hoạch được {GetColoredText($"+{buffValues2[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref LinhLucHoTrao value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}