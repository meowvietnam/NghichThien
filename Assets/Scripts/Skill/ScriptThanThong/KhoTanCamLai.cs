using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(KhoTanCamLai), menuName = "Skill/Thần Thông/" + nameof(KhoTanCamLai))]

public class KhoTanCamLai : ThanThong
{
    [SerializeField] private int[] kinhDaoReset;
    [SerializeField] private int[] healValues;
    [SerializeField] private int[] healPerChiThuyValues;
    [SerializeField] private float delayTimeEffect = 1f;

    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (currentTurn == null) return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is KhoTanCamLai))
        {
            currentTurn.Healing((int)currentTurn.characterStatsTemp.hpMax);
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);

        }
    }

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TriLieu, healValues[GetIndexLevelThanThong(caster)]);

        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KinhDao, out SecureInt  kinhDao);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChiThuy, out SecureInt  chiThuy);

        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TriLieu, healPerChiThuyValues[GetIndexLevelThanThong(caster)] * chiThuy);

        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KinhDao, kinhDao);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChiThuy, chiThuy);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KinhDao, kinhDaoReset[GetIndexLevelThanThong(caster)]);

        caster.AddThanThongImpactAfterPlaySkill(this);
        caster.SetHp(1);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);
        ShowSkillEffectWorld(caster, delayTimeEffect);

    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Đưa Hp của bản thân về 1, thu hoạch được {GetColoredText($"+{healValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TriLieu)}. Tiêu hao tất cả {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChiThuy)}. Mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChiThuy)} thu hoạch được {GetColoredText($"+{healPerChiThuyValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TriLieu)}, Làm mới {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KinhDao)} thành {GetColoredText($"{kinhDaoReset[level - 1]}", "red")} tầng. Hiệp sau hồi toàn bộ Hp";
    }
}
