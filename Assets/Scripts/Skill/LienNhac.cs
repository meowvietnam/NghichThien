using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(LienNhac), menuName = "Skill/Thần Thông/" + nameof(LienNhac))]

public class LienNhac : ThanThong
{
    
    [SerializeField]
    private int require = 5;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Tran, require);
        caster.isSkipTieuTanLinhKhi = true;

    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Tran, out SecureInt  tran);

        return tran>= require;
    }
    

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Tiêu hao {GetColoredText($"{require}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)} thì kết thúc hiệp này không cần tiêu tán linh khí";

    }
}
