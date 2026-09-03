using LongDH.LongDHModules;
using Newtonsoft.Json;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(VanTuongTrienThanQuyet))]
public class VanTuongTrienThanQuyet : CongPhap, IMemorySerializable<VanTuongTrienThanQuyet>
{
    [SerializeField]
    private int[] buffValues;
    [SerializeField]
    private int[] requireActive;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int require = requireActive[CurrentLevelCongPhapInCombat(caster) - 1];
        int trienNhieu = buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
        int dame = target.GetDameBuffForTarget();
        if (CheckIsEqualETypeSkill(caster) && dame >= require)
        {
            target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu, trienNhieu);
        }
    }

    public override string GetDesc(int level)
    {
        return $"Nếu sử dụng {GetTextTypeSkill()} hệ thần thông gây {GetColoredText($"{requireActive[level - 1]} điểm", "red")} tổn thương thì đối phương nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref VanTuongTrienThanQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}