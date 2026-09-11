using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThuHuyet), menuName = "Skill/Thần Thông/" + nameof(ThuHuyet))]
public class ThuHuyet : ThanThong, IMemorySerializable<ThuHuyet>
{
    [SerializeField] private int[] hapThu;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KietSuc, 1);
        foreach (var item in caster.DictHapThuLinhKhi(hapThu[GetIndexLevelThanThong(caster)]))
        {
            caster.HapThuLinhKhi(item.Key, item.Value);
        }

        PlayVisualSkill(caster, target);
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, 1.5f);
    }

    public override string GetDesc(int level)
    {
        return
            $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Hấp thụ {GetColoredText($"ngẫu nhiên {hapThu[level - 1]} điểm", "red")} linh khí. Bạn bị {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KietSuc)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThuHuyet value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}