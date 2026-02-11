using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(VanVuKiemPhap), menuName = "Skill/Thần Thông/" + nameof(VanVuKiemPhap))]

public class VanVuKiemPhap : ThanThong
{


    [SerializeField] protected int[] dames;
    [SerializeField] protected int addKiemKhi = 1;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        target.TakeDameByThanThong(caster, new List<int> { dames[GetIndexLevelThanThong(caster)] }, false);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HuyenVu, out int huyenVu);
        if(huyenVu > 0)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, huyenVu * addKiemKhi);

        }

    }
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.KiemY)} Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HuyenVu)} thì thu hoạch được {GetColoredText($"+{addKiemKhi}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}";
    }

   
}
