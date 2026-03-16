using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(KiemDienThienDaoTien))]

public class KiemDienThienDaoTien : KiemDienNhanDiaThien
{
    [SerializeField]
    private int[] buffValues2;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, buffValues2[CurrentLevelCongPhapInCombat(caster) - 1]);

    }
    public override string GetDesc(int level)
    {
        return $"{base.GetDesc(level)}. Mỗi lần phát động {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} thu hoạch được {GetColoredText($"+{buffValues2[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}";
    }
    public override void InitPosCallExecute(CharacterCombat caster)
    {
        base.InitPosCallExecute(caster);
        Observer.AddObserver(EventList.Combat.KIEM_KHI, ExecuteParams);

    }
}
