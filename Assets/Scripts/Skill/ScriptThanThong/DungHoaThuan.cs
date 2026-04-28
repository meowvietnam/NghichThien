using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(DungHoaThuan), menuName = "Skill/Thần Thông/" + nameof(DungHoaThuan))]

public class DungHoaThuan : ThanThong
{
    [SerializeField]
    private int[] buffValues = new int[]
     {
        2,4,6,8,15
     };
    [SerializeField]
    private int[] giamTonThuong = new int[]
     {
        1,1,1,1,1
     };
    [SerializeField]
    private int require = 2;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Tran, require);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buffValues[GetIndexLevelThanThong(caster)]);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.GiamTonThuong, giamTonThuong[GetIndexLevelThanThong(caster)]);


    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Tran, out SecureInt tran);
        if(tran>= require && base.IsCanUse(caster, target))
        {
            return true;
        }    
        return false;
    }
    public override string GetDesc(int level)
    {
        return $"Tiêu hao {GetColoredText($"{require}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)}.Thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} và {GetColoredText($"+{giamTonThuong[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.GiamTonThuong)}";

    }
}
