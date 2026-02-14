using UnityEngine;
using LongDH.LongDHModules;
using System.Linq;

[CreateAssetMenu(fileName = nameof(KhuDangThuat), menuName = "Skill/Thần Thông/" + nameof(KhuDangThuat))]
public class KhuDangThuat : ThanThong
{
    [SerializeField] private int[] trienNhieu;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ObserverEffect);
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG, ObserverEffect2);

    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        if (currentTurn == null) return;

        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is KhuDangThuat))
        {
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);

        }


    }
    private void ObserverEffect2(params object[] data)
    {
        CharacterCombat charSendDame = (CharacterCombat)data[0];

        CharacterCombat charTakeDame = (CharacterCombat)data[1];
        if (charTakeDame.specialThanThongImpactAfterPlaySkill.Any(x => x is KhuDangThuat))
        {
            charSendDame.AddBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu, trienNhieu[GetIndexLevelThanThong(charTakeDame)]);
            charTakeDame.RemoveAllThanThongImpactAfterPlaySkill(this);

        }



    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Ở hiệp tiếp theo của đối thủ bản thân bị tổn thương thì đối thủ nhận phải {GetColoredText($"+{trienNhieu[level - 1]} điểm", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)}";
    }
}
