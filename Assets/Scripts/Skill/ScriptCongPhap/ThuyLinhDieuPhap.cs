using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThuyLinhDieuPhap))]
public class ThuyLinhDieuPhap : CongPhap, IMemorySerializable<ThuyLinhDieuPhap>
{
    [SerializeField] private int[] buffValues = new int[] { 1, 1, 2, 3, 4 };

    public virtual void ExecuteParams2(params object[] data)
    {
        CharacterCombat caster = (CharacterCombat)data[0];
        CharacterCombat target = (CharacterCombat)data[1];
        // Tìm trong danh sách trang bị của caster
        if (GetCongPhapMono(caster.congPhapEquip) != null)
        {
            Execute2(caster, target, data);
        }
    }

    public virtual void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (caster.thanThongOldUse == null || caster.thanThongCurrentUse == null)
            return;
        if (CheckIsEqualETypeSkill(caster) &&
            CombatManager.IsLienKich(caster.thanThongCurrentUse.thanThong, caster.thanThongOldUse.thanThong))
        {
            caster.AddCPDynamic(GetCongPhapMono(caster.congPhapEquip));
        }
    }

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int count = caster.congPhapDynamic.Count(x => x.congPhap is ThuyLinhDieuPhap);
        int buff = buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.SucLuc, buff * count);
        RemoveCongPhap(caster);
    }

    public override string GetDesc(int level)
    {
        return
            $"Khi sử dụng {GetTextLinhKhi(ELinhKhi.Thuy)} hệ thần thông, nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Kim)} hệ thần thông thì hiệp sau thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucLuc)} (Có thể cộng dồn)";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        RemoveCongPhap(caster);
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams2);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThuyLinhDieuPhap value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}