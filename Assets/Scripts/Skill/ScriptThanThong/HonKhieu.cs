using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(HonKhieu), menuName = "Skill/Thần Thông/" + nameof(HonKhieu))]
public class HonKhieu : ThanThong, IMemorySerializable<HonKhieu>
{
    [SerializeField] private int[] dames = new int[] { 20, 30, 45, 90, 180 };
    [SerializeField] private int[] buffValues = new int[] { 3, 3, 3, 3, 3 };
    [SerializeField] private float delayTimeEffect = 1f;
    [SerializeField] private float cameraSize = 5;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Hon, out SecureInt hon);
        int dame = dames[GetIndexLevelThanThong(caster)] + hon * buffValues[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        PlayVisualSkill(caster, target);
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectParticleBase(caster, delayTimeEffect, cameraSize);
    }

    protected override (Action<Transform, Transform>, Action<Transform, Transform>) GetActionControlEffectParticleBase()
    {
        // nếu caster ng nhận effec là enemy  thì 
        return ((childEffect, childPrefab) =>
        {
            base.GetActionControlEffectParticleBase().Item1?.Invoke(childEffect, childPrefab);
            childEffect.transform.localEulerAngles = new Vector3(childPrefab.transform.localEulerAngles.x,
                childPrefab.transform.localEulerAngles.y, childPrefab.transform.localEulerAngles.z);
        }, (childEffect, childPrefab) =>
        {
            base.GetActionControlEffectParticleBase().Item2?.Invoke(childEffect, childPrefab);
            childEffect.transform.localEulerAngles = new Vector3(childPrefab.transform.localEulerAngles.x,
                childPrefab.transform.localEulerAngles.y * -1, childPrefab.transform.localEulerAngles.z);
        });
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(GetEKeywordBuffSkillQuy(caster.characterStatsTemp.level),
            out SecureInt quy);
        if (quy > 0 && base.IsCanUse(caster, target))
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

    public override string GetDesc(int level)
    {
        return
            $"Bản thân có {GetKeyWordBuffOrDeBuff(GetEKeywordBuffSkillQuyWithDaiCanhGioi(level))} mới có thể sử dụng. Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Hon)} thì tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HonKhieu value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}