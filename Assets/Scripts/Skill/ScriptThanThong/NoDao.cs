using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(NoDao), menuName = "Skill/Thần Thông/" + nameof(NoDao))]
public class NoDao : ThanThong, BaseSkill.LienKich, IMemorySerializable<NoDao>
{
    [SerializeField]
    protected int countAttack = 3;
    [SerializeField]
    protected int countAttackLienKich = 2;
    [SerializeField]
    private float delayTimeEffect = 1f;
    [SerializeField]
    protected int[] damePerCountAttack;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        List<int> dames = new List<int>();
        for (int i = 0; i < countAttack; i++)
        {
            dames.Add(damePerCountAttack[GetIndexLevelThanThong(caster)]);
        }

        target.CaculateDameByThanThong(caster, dames, eTypeSkill);
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
        return $"Tạo thành {GetColoredText($"{countAttack} lần {damePerCountAttack[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)} Nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Kim)} hệ thần thông thì tăng thêm {GetColoredText($"{countAttackLienKich} lần", "red")}";
    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        List<int> dames = new List<int>();
        for (int i = 0; i < countAttackLienKich; i++)
        {
            dames.Add(damePerCountAttack[GetIndexLevelThanThong(caster)]);
        }

        target.CaculateDameByThanThong(caster, dames, eTypeSkill);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref NoDao value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}