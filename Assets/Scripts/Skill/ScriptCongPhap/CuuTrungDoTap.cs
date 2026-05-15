using LongDH.LongDHModules;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(CuuTrungDoTap))]
public class CuuTrungDoTap : CongPhap, IMemorySerializable<CuuTrungDoTap>
{
    [SerializeField]
    private int[] countDebuff;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (caster.thanThongCurrentUse == null || caster.thanThongOldUse == null)
            return;
        if (CheckIsEqualETypeSkill(caster) && CombatManager.IsLienKich(caster.thanThongCurrentUse.thanThong, caster.thanThongOldUse.thanThong))
        {
            int count = countDebuff[CurrentLevelCongPhapInCombat(caster) - 1];
            for (int i = 0; i < count; i++)
            {
                var debuff = eDebuffs[CombatStatic.IndexRandom(0, eDebuffs.Count)];
                target.AddBuffAndDebuffCount(debuff, 1);
            }

            Debug.Log("co voi vao CuuTrungDoTap");
        }
    }

    public override string GetDesc(int level)
    {
        return $"Sử dụng {GetTextTypeSkill()} thần thông, nếu trước đó dùng Thủy hệ thần thông thì đối phương nhận phải {GetColoredText($"+{countDebuff[level - 1]}", "red")} lần ngẫu nhiên trạng thái tiêu cực";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref CuuTrungDoTap value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}