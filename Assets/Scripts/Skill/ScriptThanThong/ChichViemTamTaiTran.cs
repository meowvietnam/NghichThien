using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ChichViemTamTaiTran), menuName = "Skill/Thần Thông/" + nameof(ChichViemTamTaiTran))]
public class ChichViemTamTaiTran : TranPhapTT
{
    [SerializeField]
    private int[] buffValues = new int[]
    {
        2,3,4,5,6
    };
    [SerializeField]
    private int[] dames = new int[]
    {
        4,8,12,16,20
    };
    [SerializeField]
    private int kiemKhiRequire = 4;
    [SerializeField]
    private int thieuDotRequire = 5;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, kiemKhiRequire);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, thieuDotRequire);

        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect);


    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out SecureInt kiemKhi);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThieuDot, out SecureInt thieuDot);

        if (!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong.Id == this.Id)) && kiemKhi >= kiemKhiRequire && thieuDot >= thieuDotRequire && base.IsCanUse(caster, target))
        {
            return true;
        }
        return false;
    }
    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, buffValues[GetIndexLevelThanThong(caster)]);
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, buffValues[GetIndexLevelThanThong(caster)]);
        target.TakeDameByStatic(caster,new System.Collections.Generic.List<int> { dames[GetIndexLevelThanThong(caster)] } , false, new ETypeSkill[] {ETypeSkill.Hoa});
    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Tiêu hao {GetColoredText($"{kiemKhiRequire}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} và {GetColoredText($"{thieuDotRequire}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}. Kết thúc hiệp, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}, đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}. Đồng thời tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} {GetTextLinhKhi(ELinhKhi.Hoa)} hệ tổn thương";
    }
}
