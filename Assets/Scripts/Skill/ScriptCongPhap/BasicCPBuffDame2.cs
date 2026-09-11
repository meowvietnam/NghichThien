using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(BasicCPBuffDame2))]
public class BasicCPBuffDame2 : CongPhap, IMemorySerializable<BasicCPBuffDame2>
{
    [SerializeField] private int[] dameBuff;
    [SerializeField] private ELinhKhi eLinhKhi;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // Kiểm tra hệ của Skill đang dùng có khớp với Công Pháp (SO) này không
        if (caster.thanThongCurrentUse == null || caster.thanThongOldUse == null)
            return;
        if (CheckIsEqualETypeSkill(caster) &&
            CombatManager.IsLienKich(caster.thanThongCurrentUse.thanThong, caster.thanThongOldUse.thanThong))
        {
            target.AddDamageData(new List<int> { dameBuff[CurrentLevelCongPhapInCombat(caster) - 1] });
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Khi sử dụng {GetTextLinhKhi(eLinhKhi)} hệ thần thông sát thương, nếu trước đó dùng {GetTextLinhKhi(CombatManager.linhKhiLienKich[eLinhKhi])} thì sát thương gây ra {GetColoredText($"+{dameBuff[level - 1]}", "red")}";
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

    public void Deserialize(IReader reader, ref BasicCPBuffDame2 value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}