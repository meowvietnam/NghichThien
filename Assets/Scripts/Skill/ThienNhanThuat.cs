using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class ThienNhanThuat : ThanThong
{
    [SerializeField] private int requireKiemKhi = 2;
    [SerializeField] private int countAttack = 8;
    [SerializeField] private int damePerCountAttack = 9;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(IsCanUse(caster,target))
        {
            caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi,requireKiemKhi);
            List<int> dames = new List<int>();
            for (int i = 0; i < countAttack; i++)
            {
                dames.Add(damePerCountAttack);
            }
            target.TakeDameCaculate(caster, dames, false);

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
    public override string GetDesc()
    {
        return $"Tiêu hao {requireKiemKhi} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} tạo thành {GetColoredText($"{countAttack} lần {damePerCountAttack}", "red")} điểm sát thương";

    }


}
