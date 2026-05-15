using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Skill;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(CuLinhTrao), menuName = "Skill/Thần Thông/" + nameof(CuLinhTrao))]
public class CuLinhTrao : ThanThong, IMemorySerializable<CuLinhTrao>
{
    [SerializeField]
    private int[] dames;
    [SerializeField]
    private int[] dameBuffs;
    [SerializeField]
    private float delayTimeEffect = 0.5f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        int countUse = GetListSkillUseCheck(caster).Count(data => data.thanThong is CuLinhTrao);
        target.CaculateDameByThanThong(caster, new List<int>{dame + (countUse * dameBuffs[GetIndexLevelThanThong(caster)])}, eTypeSkill);
        ShowSkillEffectWorld(target, delayTimeEffect);
    }

    protected virtual List<CharacterCombat.PlaySkillData> GetListSkillUseCheck(CharacterCombat caster)
    {
        return caster.thanThongUseThisTurn;
    }

    protected virtual string GetLogicText()
    {
        return "Trong hiệp này";
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương,{GetColoredText($"{GetLogicText()}", "red")} mỗi lần sử dụng {GetColoredText($"[Cự Linh Trảo]", "green")} thì tổn thương {GetColoredText($"+{dameBuffs[level - 1]}", "red")} điểm sát thương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref CuLinhTrao value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}