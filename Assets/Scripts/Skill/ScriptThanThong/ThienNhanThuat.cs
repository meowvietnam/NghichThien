using System.Collections.Generic;
using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThienNhanThuat), menuName = "Skill/Thần Thông/" + nameof(ThienNhanThuat))]
public class ThienNhanThuat : ThanThong
{
    [SerializeField] protected int requireKiemKhi = 2;
    [SerializeField] protected int[] countAttack;
    [SerializeField] protected int[] damePerCountAttack;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, requireKiemKhi);
        List<int> dames = new List<int>();
        for (int i = 0; i < countAttack[GetIndexLevelThanThong(caster)]; i++)
        {
            dames.Add(damePerCountAttack[GetIndexLevelThanThong(caster)]);
        }
        target.CaculateDameByThanThong(caster, dames, eTypeSkill, false);
        ShowSkillEffectWorld(target, delayTimeEffect);

    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        if (caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out SecureInt  kiemKhi) && base.IsCanUse(caster, target))
        {
            return kiemKhi >= requireKiemKhi;
        }

        return false; // không có KiemKhi thì coi như = 0
    }
    public override string GetDesc(int level)
    {
        return $"Tiêu hao {requireKiemKhi} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} tạo thành {GetColoredText($"{countAttack[level - 1]} lần {damePerCountAttack[level - 1]}", "red")} điểm sát thương";

    }


}
