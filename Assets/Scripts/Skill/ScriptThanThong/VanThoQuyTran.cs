using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(VanThoQuyTran), menuName = "Skill/Thần Thông/" + nameof(VanThoQuyTran))]
public class VanThoQuyTran : ThanThong, IMemorySerializable<VanThoQuyTran>
{
    [SerializeField] private int[] buffValues = new int[] { 1, 3, 7, 20, 40 };
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Tran, caster.khiTho * buffValues[GetIndexLevelThanThong(caster)]);
        caster.TieuTanLinhKhi(ELinhKhi.Tho, caster.khiTho);
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
        return
            $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Tiêu tán tất cả {GetTextLinhKhi(ELinhKhi.Tho)} hệ linh khí, mỗi tiêu tán một điểm thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref VanThoQuyTran value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}