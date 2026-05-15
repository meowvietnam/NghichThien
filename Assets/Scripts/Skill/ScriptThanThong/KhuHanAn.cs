using LongDH.LongDHModules;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(KhuHanAn), menuName = "Skill/Thần Thông/" + nameof(KhuHanAn))]
public class KhuHanAn : ThanThong, IMemorySerializable<KhuHanAn>
{
    [SerializeField]
    private float delayEffect = 0.5f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.ADJUST_LINHKHI, ObserverEffect);
        ShowSkillEffectWorld(caster, delayEffect);
    }

    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        int count = (int)data[2];
        if (currentTurn == null)
            return;
        if (count > 0) // hấp thụ
        {
            currentTurn.RemoveBuffAndDebuffCount(EBuffAndDeBuff.SuongGia, count);
        }
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        if (!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong is KhuHanAn)) && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override string GetDesc(int level)
    {
        return $"Trong chiến đấu, mỗi hấp thụ 1 điểm linh khí thì bản thân gỡ ra 1 tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SuongGia)}. {GetColoredText($"Mỗi cuộc chiến chỉ có thể sử dụng một lần", "red")}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KhuHanAn value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}