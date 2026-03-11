using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(NghiepHoaAn), menuName = "Skill/Thần Thông/" + nameof(NghiepHoaAn))]

public class NghiepHoaAn : ThanThong , BaseSkill.LienKich
{
    [SerializeField]
    private int[] valueDeBuffs = new int[]
   {
        1,2,3,4,5
   };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.GamePlay.LIEN_KICH, ObserverEffect);


    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (currentTurn == null) return;

        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is NghiepHoaAn))
        {
           int count = currentTurn.specialThanThongImpactAfterPlaySkill.Count(x => x is NghiepHoaAn);
            targetTurn.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, valueDeBuffs[GetIndexLevelThanThong(currentTurn)] * count);

        }


    }
    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        return;
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Trong cuộc chiến, mỗi khi phát động ngũ hành {GetKeyWord(EKeyWord.LienKich)} thì đối thủ nhận phải {GetColoredText($"+{valueDeBuffs[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)} (Có thể cộng dồn)";

    }
}
