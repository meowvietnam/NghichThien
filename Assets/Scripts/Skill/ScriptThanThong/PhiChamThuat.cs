using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(PhiChamThuat), menuName = "Skill/Thần Thông/" + nameof(PhiChamThuat))]
public class PhiChamThuat : ThanThong, IMemorySerializable<PhiChamThuat>
{
    [SerializeField]
    private int[] dames;
    [SerializeField]
    private int deTonThuong = 1;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Cham, 1);
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dame}, eTypeSkill);
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.DeTonThuong, deTonThuong);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"Tiêu hao 1 tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Cham)}, tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Đối phương nhận phải {GetColoredText($"+{deTonThuong}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DeTonThuong)}";
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        SecureInt cham = 0;
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Cham, out cham);
        return cham > 0 && base.IsCanUse(caster, target);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref PhiChamThuat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}