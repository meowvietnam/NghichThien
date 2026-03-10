using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(HuyenVuKiemTran), menuName = "Skill/Thần Thông/" + nameof(HuyenVuKiemTran))]
public class HuyenVuKiemTran : TranPhapTT
{

    [SerializeField]
    private int[] buffValues = new int[]
     {
        1,1,1,1,1
     };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, ObserverEffect);


    }
    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out SecureInt kiemKhi);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, kiemKhi * buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Kết thúc hiệp, mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
    }

}
