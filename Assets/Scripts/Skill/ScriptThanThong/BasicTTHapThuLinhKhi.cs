using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(BasicTTHapThuLinhKhi), menuName = "Skill/Thần Thông/" + nameof(BasicTTHapThuLinhKhi))]
public class BasicTTHapThuLinhKhi : ThanThong, BaseSkill.TuongSinh, IMemorySerializable<BasicTTHapThuLinhKhi>
{
    [SerializeField]
    private int[] hapThus;
    [SerializeField]
    private int[] hapThuAdds;
    [SerializeField]
    private ELinhKhi eLinhKhi;
    [SerializeField]
    private float delayTimeEffect = 1f;
    ELinhKhi linhKhiHapThu => CombatManager.TuongSinh[eLinhKhi];
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.HapThuLinhKhi(linhKhiHapThu, hapThus[GetIndexLevelThanThong(caster)]);
        PlayVisualSkill(caster, target);
    }
    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public void ExecuteTuongSinh(CharacterCombat caster, CharacterCombat target)
    {
        caster.HapThuLinhKhi(linhKhiHapThu, hapThuAdds[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Hấp thụ {GetColoredText($"+{hapThus[level - 1]} điểm", "red")} {GetTextLinhKhi(linhKhiHapThu)}. Nếu {GetKeyWord(EKeyWord.TuongSinh)} hấp thụ thêm {GetColoredText($"+{hapThuAdds[level - 1]} điểm", "red")} {GetTextLinhKhi(linhKhiHapThu)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref BasicTTHapThuLinhKhi value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}