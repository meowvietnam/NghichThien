using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThieuDotHoaAn), menuName = "Skill/Thần Thông/" + nameof(ThieuDotHoaAn))]

public class ThieuDotHoaAn : ThanThong
{
    [SerializeField] private int[] countThieuDot;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ObserverEffect);
        Observer.AddObserver(EventList.GamePlay.CACULATE_DAME_BY_THANTHONG, ObserverEffect2);

    }
    private void ObserverEffect(params object[] data)
    {
        Debug.Log("có gọi vào LinhQuangANRemoveCheck1");
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        if (currentTurn == null) return;

        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is ThieuDotHoaAn))
        {
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);

        }


    }
    private void ObserverEffect2(params object[] data)
    {
        Debug.Log("có gọi vào LinhQuangANRemoveCheck2");
        CharacterCombat charSendDame = (CharacterCombat)data[0];

        CharacterCombat charTakeDame = (CharacterCombat)data[1];
        if (charTakeDame.specialThanThongImpactAfterPlaySkill.Any(x => x is ThieuDotHoaAn))
        {
            charTakeDame.RemoveAllThanThongImpactAfterPlaySkill(this);
            charSendDame.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, countThieuDot[GetIndexLevelThanThong(charTakeDame)]);
        }



    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Ở hiệp tiếp theo của đối thủ bản thân bị tổn thương thì đối phương nhận phải {GetColoredText($"+{countThieuDot[level - 1]} điểm", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}";
    }
}
