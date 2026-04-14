using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ChichMachChi), menuName = "Skill/Thần Thông/" + nameof(ChichMachChi))]

public class ChichMachChi : ThanThong
{
    [SerializeField]
    private int[] requires = new int[]
   {
        3,3,4,5,5
   };
    [SerializeField]
    private int[] dameValues = new int[]
     {
        24,34,55,100,200
     };
    [SerializeField] private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        HauKeVoLuc(caster);

        int dame = dameValues[GetIndexLevelThanThong(caster)];

        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);

        target.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.ADJUST_LINHKHI, ObserverEffect);
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect2);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

   
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        int count = (int)data[2];
        if (currentTurn == null) return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is ChichMachChi))
        {
            if (Mathf.Abs(count) >= requires[GetIndexLevelThanThong(currentTurn)])
            {
                CombatStatic.ThieuDot(null, currentTurn);

            }

        }
        


    }
    private void ObserverEffect2(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];

        if (currentTurn == null) return;

        currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);


    }
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}.Tạo thành {GetColoredText($"+{dameValues[level - 1]}", "red")} điểm sát thương. Trong hiệp đối thủ, mỗi hấp thụ hoặc tiêu tán {GetColoredText($"{requires[level - 1]}", "red")} điểm linh khí thì phát động trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}";

    }
}
