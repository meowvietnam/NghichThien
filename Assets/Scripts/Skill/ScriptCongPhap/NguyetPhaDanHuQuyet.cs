using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(NguyetPhaDanHuQuyet))]
public class NguyetPhaDanHuQuyet : CongPhap, IMemorySerializable<NguyetPhaDanHuQuyet>
{
    [SerializeField]
    private int[] buffValues = new int[]{22, 26, 30, 34, 40};
    [SerializeField]
    private int percent = 30;
    [SerializeField]
    private int[] debuffValues = new int[]{1, 2, 3, 4, 5};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (CheckIsEqualETypeSkill(caster))
        {
            target.AddDamageData(new List<int>{(int)(target.GetDameBuffForTarget() * buffValues[CurrentLevelCongPhapInCombat(caster) - 1] / 100f)});
        }
    }

    public virtual void ExecuteParams2(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        // Kiểm tra caster không null và thỏa mãn điều kiện sử dụng
        if (caster != null && IsCanUse(caster))
        {
            Execute2(caster, target, data);
        }
    }

    public void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (CombatStatic.IsActiveWithPercent(percent))
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ThanThuong, debuffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
        }
    }

    public override string GetDesc(int level)
    {
        return $"{GetTextTypeSkill(ETypeSkill.Than)} hệ thần thông tổn thương tăng lên {GetColoredText($"{buffValues[level - 1]}%", "red")} nhưng khi sử dụng thần thông có  {GetColoredText($"{percent}%", "red")} nhận phải {GetColoredText($"+{debuffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThanThuong)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams);
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams2);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref NguyetPhaDanHuQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}