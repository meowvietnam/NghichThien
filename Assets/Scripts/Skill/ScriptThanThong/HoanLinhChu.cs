using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(HoanLinhChu), menuName = "Skill/Thần Thông/" + nameof(HoanLinhChu))]
public class HoanLinhChu : ThanThong, IMemorySerializable<HoanLinhChu>
{
    [SerializeField] private int[] tieuHaos = new int[] { 20, 32, 42, 60, 120 };
    [SerializeField] private float delayTimeEffect = 1f;
    [SerializeField] private float cameraSize = 5;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.QuyCuu, out SecureInt quyCuu);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.QuyCuu, quyCuu);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Hon, tieuHaos[GetIndexLevelThanThong(caster)]);
        caster.AddBuffAndDebuffCount(GetEKeywordBuffSkillQuy(caster.characterStatsTemp.level), 1);
        CombatStatic.QuyTon(caster, target);
        PlayVisualSkill(caster, target);
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectParticleBase(caster, delayTimeEffect, cameraSize);
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.QuyCuu, out SecureInt quyCuu);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Hon, out SecureInt hon);
        if (quyCuu > 0 && hon >= tieuHaos[GetIndexLevelThanThong(caster)] && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override EBuffAndDeBuff[] GetEKeywordBuffSkillHave(int level)
    {
        // 1. Get the list from the base class first
        List<EBuffAndDeBuff> ebuffAndDebuffList = new List<EBuffAndDeBuff>(base.GetEKeywordBuffSkillHave(level));
        int daiCanhGioi = GetDaiCanhGioi(level);
        // 2. Use a switch for cleaner, more readable logic
        switch (daiCanhGioi)
        {
            case 1:
                ebuffAndDebuffList.Add(EBuffAndDeBuff.QuyLinh);
                break;
            case 2:
                ebuffAndDebuffList.Add(EBuffAndDeBuff.QuyBinh);
                break;
            case 3:
                ebuffAndDebuffList.Add(EBuffAndDeBuff.QuyTuong);
                break;
            case 4:
                ebuffAndDebuffList.Add(EBuffAndDeBuff.QuyTon);
                break;
            case 5:
                ebuffAndDebuffList.Add(EBuffAndDeBuff.QuyDe);
                break;
        }

        // 3. Return as an array
        return ebuffAndDebuffList.ToArray();
    }

    public EBuffAndDeBuff GetEKeywordBuffSkillQuy(int level)
    {
        int daiCanhGioi = GetDaiCanhGioi(level);
        return GetEKeywordBuffSkillQuyWithDaiCanhGioi(daiCanhGioi);
    }

    public EBuffAndDeBuff GetEKeywordBuffSkillQuyWithDaiCanhGioi(int daiCanhGioi)
    {
        switch (daiCanhGioi)
        {
            case 1:
                return EBuffAndDeBuff.QuyLinh;
            case 2:
                return EBuffAndDeBuff.QuyBinh;
            case 3:
                return EBuffAndDeBuff.QuyTuong;
            case 4:
                return EBuffAndDeBuff.QuyTon;
            case 5:
                return EBuffAndDeBuff.QuyDe;
            // Always include a default or a final return 
            // to handle cases where daiCanhGioi is 0 or > 5
            default:
                return EBuffAndDeBuff.None;
        }
    }

    public override string GetDesc(int daiCanhGioi)
    {
        return
            $"Bản thân có đủ thì tiêu hao {GetColoredText($"{tieuHaos[daiCanhGioi - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Hon)} đánh thức {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.QuyCuu)} triệu hồi {GetKeyWordBuffOrDeBuff(GetEKeywordBuffSkillQuyWithDaiCanhGioi(daiCanhGioi))}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HoanLinhChu value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}