using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(PhuongLinhYeuHoa), menuName = "Skill/Thần Thông/" + nameof(PhuongLinhYeuHoa))]
public class PhuongLinhYeuHoa : ThanThong, BaseSkill.LienKich, IMemorySerializable<PhuongLinhYeuHoa>
{
    [SerializeField]
    private int[] damePerCountAttack = new int[]{8, 11, 14, 18, 25};
    [SerializeField]
    private int countAttack = 2;
    [SerializeField]
    private float delayEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        List<int> dames = new List<int>();
        for (int i = 0; i < countAttack; i++)
        {
            dames.Add(damePerCountAttack[GetIndexLevelThanThong(caster)]);
        }

        target.CaculateDameByThanThong(caster, dames, eTypeSkill, false);
        ShowSkillEffectParticleBase(caster, delayEffect, 5f);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"{countAttack} lần {damePerCountAttack[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)} Nếu lần trước sử dụng là {GetTextLinhKhi(ELinhKhi.Moc)} hệ thần thông thì tăng lên {GetColoredText($"{countAttack}*X lần ", "red")} (X là số lần sử dụng thần thông [Nhóm Lửa])";
    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        int count = caster.specialThanThongImpactAfterPlaySkill.Count(x => x is NhomLua);
        List<int> dames = new List<int>();
        for (int i = 0; i < count * countAttack; i++)
        {
            dames.Add(damePerCountAttack[GetIndexLevelThanThong(caster)]);
        }

        target.CaculateDameByThanThong(caster, dames, eTypeSkill, false);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref PhuongLinhYeuHoa value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}