using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TramThienKiem), menuName = "Skill/Thần Thông/" + nameof(TramThienKiem))]

public class TramThienKiem : ThanThong
{
    [SerializeField]
    private int[] dames = new int[]
    {
        19,27,38,74,148
    };
   
    [SerializeField] private int require = 2;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        int dame = dames[GetIndexLevelThanThong(caster)];
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, require);
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);

    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out SecureInt kiemKhi);
        if(kiemKhi >= require && base.IsCanUse(caster, target))
        {
            return true;
        }    
        return false;
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.KiemY)} Tiêu hao {GetColoredText($"{require}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm {GetKeyWord(EKeyWord.ChanThucTonThuong)}";

    }
}
