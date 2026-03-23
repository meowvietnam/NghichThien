using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(CuongKimMocThuyHoaTho), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(CuongKimMocThuyHoaTho))]

public class CuongKimMocThuyHoaTho : NgoDaoCombat
{
    [SerializeField] private ELinhKhi elinhKhi;
    [SerializeField] private int dameBf = 6;
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        Debug.Log("Excute ngo dao CuongKimMocThuyHoaTho");
        if (caster.thanThongCurrentUse == null || caster.thanThongOldUse == null) return;
        Debug.Log("is equal type " +BaseSkill.CheckIsEqualETypeSkill(caster, eTypeRequire));
        Debug.Log("is lien kich " + CombatManager.IsLienKich(caster.thanThongCurrentUse.thanThong, caster.thanThongOldUse.thanThong));

        if (BaseSkill.CheckIsEqualETypeSkill(caster, eTypeRequire) && CombatManager.IsLienKich(caster.thanThongCurrentUse.thanThong, caster.thanThongOldUse.thanThong))
        {
            Debug.Log("Excute ngo dao AddDame");
            target.AddDamageData(new System.Collections.Generic.List<int> { dameBf });

        }
    }
    
    
    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);

    }

    public override string GetDes()
    {
        return $"Sử dụng {BaseSkill.GetTextLinhKhi(elinhKhi)} hệ thần thông gây tổn thương, nếu trước đó dùng {BaseSkill.GetTextLinhKhi(CombatManager.linhKhiLienKich[elinhKhi])} thì tăng lên {BaseSkill.GetColoredText($"+{dameBf}", "red")} điểm sát thương";
    }
}
