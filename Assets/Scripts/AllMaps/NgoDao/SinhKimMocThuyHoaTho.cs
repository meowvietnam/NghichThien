using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(SinhKimMocThuyHoaTho), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(SinhKimMocThuyHoaTho))]
public class SinhKimMocThuyHoaTho : NgoDaoCombat
{
    [SerializeField] private ELinhKhi elinhKhi;
    [SerializeField] private int countLinhKhi = 2;


    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.HapThuLinhKhi(elinhKhi, countLinhKhi);
    }

    public override string GetDes()
    {
        return $"Bắt đầu hiệp, hấp thụ {BaseSkill.GetColoredText($"+{countLinhKhi}", "red")} điểm {BaseSkill.GetTextLinhKhi(elinhKhi)} hệ linh khí";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);
    }

}
