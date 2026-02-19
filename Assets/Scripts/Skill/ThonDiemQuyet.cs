using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThonDiemQuyet), menuName = "Skill/Thần Thông/" + nameof(ThonDiemQuyet))]

public class ThonDiemQuyet : ThanThong
{
    [SerializeField] private int require = 3;
    [SerializeField] private int[] healValues = new int[]
    {
        1,2,3,4,5
    };
    [SerializeField]
    private int[] dameBuffs = new int[]
    {
        2,3,4,5,6
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, require);
        caster.Healing(healValues[GetIndexLevelThanThong(caster)]);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG, ObserverEffect);



    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (currentTurn == null) return;
        if(!CheckIsEqualETypeSkill(currentTurn)) return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is ThonDiemQuyet))
        {
            int count = currentTurn.specialThanThongImpactAfterPlaySkill.Count(x => x is ThonDiemQuyet);
            int dame = dameBuffs[GetIndexLevelThanThong(currentTurn)] * count;
            targetTurn.AddDamageData(new System.Collections.Generic.List<int> { dame } ,true);
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);

        }


    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThieuDot, out int thieuDot);
        if(thieuDot >= require)
        {
            return true;
        }    
        return false;
    }



    public override string GetDesc(int level)
    {
        return $"Bản thân gỡ ra {GetColoredText($"{require}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}, khôi phục {GetColoredText($"+{healValues[level - 1]}", "red")} điểm HP. Lần tiếp theo tạo thành {GetTextTypeSkill()} hệ thần thông tổn thương {GetColoredText($"+{dameBuffs[level - 1]}", "red")} (Có thể cộng dồn)";

    }
}
