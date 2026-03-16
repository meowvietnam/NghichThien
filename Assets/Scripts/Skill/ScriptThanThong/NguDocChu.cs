using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(NguDocChu), menuName = "Skill/Thần Thông/" + nameof(NguDocChu))]
public class NguDocChu : ThanThong
{
    [SerializeField] private int[] trungDocPerRequire;
    [SerializeField] private int require = 10;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);



    }
    protected void ObserverEffect(params object[] data)
    {
        CharacterCombat caster = (CharacterCombat)data[0];

        CharacterCombat target = (CharacterCombat)data[1];

       
        if (caster.specialThanThongImpactAfterPlaySkill.Any(x => x is NguDocChu))
        {

            SecureInt trungDoc = 0;
            target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrungDoc, out trungDoc);
            int count = (trungDoc / require);
            target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrungDoc, count * trungDocPerRequire[GetIndexLevelThanThong(caster)]);
            caster.RemoveAllThanThongImpactAfterPlaySkill(this);

        }

    }    

    public override string GetDesc(int level)   
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Bắt đầu hiệp, đối phương mỗi có {GetColoredText($"{require}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)} thì nhận phải {GetColoredText($"+{trungDocPerRequire[level-1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)}";
    }
}
