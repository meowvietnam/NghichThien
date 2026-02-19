using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThienNhanThuat), menuName = "Skill/Thần Thông/" + nameof(ThienNhanThuat))]
public class ThienNhanThuat : ThanThong
{
    [SerializeField] protected int requireKiemKhi = 2;
    [SerializeField] protected int[] countAttack;
    [SerializeField] protected int[] damePerCountAttack;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(IsCanUse(caster,target))
        {
            caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi,requireKiemKhi);
            List<int> dames = new List<int>();
            for (int i = 0; i < countAttack[GetIndexLevelThanThong(caster)]; i++)
            {
                dames.Add(damePerCountAttack[GetIndexLevelThanThong(caster)]);
            }
            target.TakeDameByThanThong(caster, dames, false);

        }

    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        if (caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out int kiemKhi))
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
