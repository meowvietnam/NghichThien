using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThoTangLuanChuyenTran), menuName = "Skill/Thần Thông/" + nameof(ThoTangLuanChuyenTran))]
public class ThoTangLuanChuyenTran : TranPhapTT
{

    [SerializeField]
    private int[] buffValues = new int[]
     {
        15,20,30,50,74
     };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.GamePlay.PLAY_SKILL, ObserverEffect);


    }
    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        if (caster.thanThongCurrentUse.thanThong is HoaCat)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanTho, buffValues[GetIndexLevelThanThong(caster)]);
        }
    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Mỗi lần sử dụng thần thông [Hóa Cát] thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanTho)}";
    }
}
