using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(DoThienLietHoaTienTran), menuName = "Skill/Thần Thông/" + nameof(DoThienLietHoaTienTran))]

public class DoThienLietHoaTienTran : TranPhapTT
{
    [SerializeField]
    private int[] buffValues = new int[]
      {
        1,2,3,4,5
      };
    [SerializeField]
    private int hapThu = 2;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);


    }
    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        caster.HapThuLinhKhi(ELinhKhi.Hoa, hapThu);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, buffValues[GetIndexLevelThanThong(caster)]);
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Bắt đầu hiệp, Hấp thụ {GetColoredText($"+{hapThu}", "red")} điểm {GetTextLinhKhi(ELinhKhi.Hoa)} hệ linh khí. Đối thủ và bản thân nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}";
    }
}
