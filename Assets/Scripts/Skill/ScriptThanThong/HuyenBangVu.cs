using LongDH.LongDHModules;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(HuyenBangVu), menuName = "Skill/Thần Thông/" + nameof(HuyenBangVu))]
public class HuyenBangVu : ThanThong, IMemorySerializable<HuyenBangVu>
{
    [SerializeField]
    private int[] dames;
    [SerializeField]
    private int[] suongGias;
    [SerializeField]
    private int thuyHapThu = 5;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.AddThanThongImpactAfterPlaySkill(this);
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new List<int>{dame}, eTypeSkill);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];
        if (currentTurn == null)
            return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is HuyenBangVu))
        {
            List<HuyenBangVu> listThisSkill = currentTurn.specialThanThongImpactAfterPlaySkill.OfType<HuyenBangVu>().ToList();
            currentTurn.HapThuLinhKhi(ELinhKhi.Thuy, thuyHapThu * listThisSkill.Count);
            currentTurn.AddBuffAndDebuffCount(EBuffAndDeBuff.SuongGia, suongGias[GetIndexLevelThanThong(targetTurn)] * listThisSkill.Count);
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
        }
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"{dames[level - 1]}", "red")} điểm sát thương. Hiệp bắt đầu của đối thủ hấp thụ {GetColoredText($"+{thuyHapThu}", "red")} điểm {GetTextTypeSkill()} hệ linh khí. Đồng thời, cũng nhận phải {GetColoredText($"+{suongGias[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SuongGia)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HuyenBangVu value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}