using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(DaiNguHanhTran), menuName = "Skill/Thần Thông/" + nameof(DaiNguHanhTran))]

public class DaiNguHanhTran : TranPhapTT
{
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ObserverEffect);


    }
    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        Pool.Get<CombatManager>().transform.GetComponentInChildren<PlayerUICombat>().linhKhiSelectUI.SetActive(true);
    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Bắt đầu hiệp, Lựa chọn một điểm linh khí bất kì để hấp thụ";
    }


}
