using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(CuuCungDienDaoTran), menuName = "Skill/Thần Thông/" + nameof(CuuCungDienDaoTran))]

public class CuuCungDienDaoTran : TranPhapTT
{

    [SerializeField] private int require = 3;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ObserverEffect);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);

        CheckBoQuaTieuTanLinhKhi(caster, target);

    }
    private void CheckBoQuaTieuTanLinhKhi(CharacterCombat caster, CharacterCombat target)
    {
        if(caster.thanThongUseThisTurn.Count <= require)
        {
            caster.isSkipTieuTanLinhKhi = true;

        }
        else
        {
            caster.isSkipTieuTanLinhKhi = false;

        }

    }    
    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        CheckBoQuaTieuTanLinhKhi(caster, target);
    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Ở hiệp này, nếu số lần sử dụng thần thông chưa nhiều hơn {GetColoredText($"{require}", "red")} lần thì nhảy qua giai đoạn tiêu tán linh khí";
    }

}
