using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThietGiapChu), menuName = "Skill/Thần Thông/" + nameof(ThietGiapChu))]

public class ThietGiapChu : ThanThong
{

    [SerializeField] protected float delayTimeEffect = 1f;

    [SerializeField]
    private int[] buffValues = new int[]
    {
        1,2,3,4,6
    };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect2);
        ShowSkillEffectWorld(caster, delayTimeEffect);

    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat target = (CharacterCombat)data[1];

        if (target == null) return;

        if (target.specialThanThongImpactAfterPlaySkill.Any(x => x is ThietGiapChu))
        {
           
            target.AddBuffAndDebuffCount(EBuffAndDeBuff.GiamTonThuong, buffValues[GetIndexLevelThanThong(currentTurn)]);


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
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Trong hiệp của đối thủ, mỗi lần bản thân nhận sát thương từ thần thông thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.GiamTonThuong)}";

    }
}
