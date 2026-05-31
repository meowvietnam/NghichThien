using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(KiemKhi), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(KiemKhi))]
public class KiemKhi : NgoDaoCombat, IMemorySerializable<KiemKhi>
{
    [SerializeField]
    private int buff = 1;
    [SerializeField]
    private int percent = 50;
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(caster.thanThongOldUse == null || caster.thanThongCurrentUse == null) return;
        if (caster.thanThongCurrentUse.thanThong == caster.thanThongOldUse.thanThong && BaseSkill.CheckIsEqualETypeSkill(caster, eTypeRequire))
        {
            if (CombatStatic.IsActiveWithPercent(percent))
            {
                caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, buff);
            }
        }
    }

    public override string GetDes()
    {
        return $"Liên tục dùng cùng một loại Kiếm hệ thần thông có {BaseSkill.GetColoredText($"{percent}%", "red")} thu hoạch được {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ObserverEffect);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KiemKhi value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}