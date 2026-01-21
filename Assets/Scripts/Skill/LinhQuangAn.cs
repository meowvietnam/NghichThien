using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu]
public class LinhQuangAn : ThanThong
{
    [SerializeField] private int[] countHapThu;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, CheckRemoveEffect);
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG, CheckRemoveEffect2);

    }
    private void CheckRemoveEffect(params object[] data)
    {
        Debug.Log("có gọi vào LinhQuangANRemoveCheck1");
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        if (currentTurn == null) return;
          
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is LinhQuangAn))
        {
            currentTurn.RemoveThanThongImpactAfterPlaySkill(this);

        }


    }
    private void CheckRemoveEffect2(params object[] data)
    {
        Debug.Log("có gọi vào LinhQuangANRemoveCheck2");

        CharacterCombat charTakeDame = (CharacterCombat)data[1];
        if (charTakeDame == null) return;
        if (charTakeDame.specialThanThongImpactAfterPlaySkill.Any(x => x is LinhQuangAn))
        {
            charTakeDame.HapThuLinhKhi(ELinhKhi.Kim, countHapThu[GetIndexLevelThanThong(charTakeDame)]);

        }

       

    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Ở hiệp tiếp theo của đối thủ bị bản thân bị tổn thương thì thu hoạch được {GetColoredText($"+{countHapThu[level - 1]} điểm", "red")} {GetTextLinhKhi(ELinhKhi.Kim)} linh khí";
    }
}
