using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TinhDieuNgungHonQuyet), menuName = "Skill/Thần Thông/" + nameof(TinhDieuNgungHonQuyet))]

public class TinhDieuNgungHonQuyet : ThanThong
{
    
    [SerializeField]
    private int[] buffValues = new int[]
    {
        3,6,9,17,35
    };
    [SerializeField]
    private int[] debuffValues = new int[]
    {
        1,2,3,4,5
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        caster.characterStatsCurrent.thanThuc.Value += buffValues[GetIndexLevelThanThong(caster)];
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TreKhi, debuffValues[GetIndexLevelThanThong(caster)]);
        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, ObserverEffect);


    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (currentTurn == null) return;

        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is TinhDieuNgungHonQuyet))
        {
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
            currentTurn.characterStatsCurrent.thanThuc.Value -= buffValues[GetIndexLevelThanThong(currentTurn)];


        }


    }


    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Bản thân nhận phải {GetColoredText($"+{debuffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TreKhi)}. Trong hiệp này tạm thời tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm thần thức";

    }
}
