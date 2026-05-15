using LongDH.LongDHModules;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(BangVuThuat), menuName = "Skill/Thần Thông/" + nameof(BangVuThuat))]
public class BangVuThuat : ThanThong, IMemorySerializable<BangVuThuat>
{
    [SerializeField]
    private int[] suongGias;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect);
        ShowSkillEffectWorld(target, delayTimeEffect);
    }

    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];
        if (currentTurn == null)
            return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is BangVuThuat))
        {
            targetTurn.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoTrao, out SecureInt hotrao);
            if (hotrao <= 0)
            {
                targetTurn.AddBuffAndDebuffCount(EBuffAndDeBuff.SuongGia, suongGias[GetIndexLevelThanThong(currentTurn)]);
            }
        }
    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, Nếu đối thủ không có {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} thì nhận phải {GetColoredText($"+{suongGias[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SuongGia)} {GetColoredText($"Mỗi cuộc chiến chỉ có thể sử dụng một lần", "red")}";
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        if (!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong is BangVuThuat)) && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref BangVuThuat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}