using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(PhanThienCong))]

public class PhanThienCong : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
       {
        1, 1 , 2,2, 3
       };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThieuDot, out SecureInt  myThieuDot);
        if(CheckIsEqualETypeSkill(caster))
        {
            int dame = buffValues[CurrentLevelCongPhapInCombat(caster)-1] * myThieuDot;
            target.AddDamageData(new System.Collections.Generic.List<int> { dame });
        }



    }
   
    public override string GetDesc(int level)
    {
        return $"Bản thân mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)} thì {GetTextLinhKhi(ELinhKhi.Hoa)} hệ thần thông sát thương {GetColoredText($"+{buffValues[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.CACULATE_DAME_BY_THANTHONG, ExecuteParams);

    }
}
