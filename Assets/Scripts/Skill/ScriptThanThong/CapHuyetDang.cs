using System.Linq;
using UnityEngine;
using LongDH.LongDHModules;
using Skill;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(CapHuyetDang), menuName = "Skill/Thần Thông/" + nameof(CapHuyetDang))]
public class CapHuyetDang : ThanThong, BaseSkill.TuongDong, IMemorySerializable<CapHuyetDang>
{
    [SerializeField] private int[] dames;
    [SerializeField] private int[] dameBuffs;
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        caster.Healing(dame);
        PlayVisualSkill(caster, target);
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(target, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return
            $"Tạo thành {GetColoredText($"{dames[level - 1]}", "red")} điểm sát thương. Đồng thời, hồi lại lượng Hp bằng sát thương gây ra. Nếu là {GetKeyWord(EKeyWord.TuongDong)} tăng lên {GetColoredText($"{dameBuffs[level - 1]}", "red")} điểm sát thương";
    }

    public void ExecuteTuongDong(CharacterCombat caster, CharacterCombat target)
    {
        int dame = dameBuffs[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill, true);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref CapHuyetDang value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}