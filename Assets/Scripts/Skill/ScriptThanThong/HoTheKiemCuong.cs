using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(HoTheKiemCuong), menuName = "Skill/Thần Thông/" + nameof(HoTheKiemCuong))]

public class HoTheKiemCuong : ThanThong
{
    
    [SerializeField]
    private int[] buffValues = new int[]
    {
        4,4,4,4,4
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out SecureInt kiemKhi);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, kiemKhi * buffValues[GetIndexLevelThanThong(caster)]);

        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, kiemKhi);

    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out SecureInt kiemKhi);
        if(kiemKhi > 0 && base.IsCanUse(caster, target))
        {
            return true;
        }
        return false;
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Tiêu hao tất cả {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}, mỗi một tầng thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";

    }
}
