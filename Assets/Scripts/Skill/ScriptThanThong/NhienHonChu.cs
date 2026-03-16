using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(NhienHonChu), menuName = "Skill/Thần Thông/" + nameof(NhienHonChu))]

public class NhienHonChu : ThanThong
{
    
    [SerializeField]
    private int[] dameValues = new int[]
     {
        5,8,11,15,30
     };
    [SerializeField]
    private int addDebuff = 1;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ThanThuong, addDebuff);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect2);
    }



    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (currentTurn == null) return;

        if(CheckIsEqualETypeSkill(currentTurn) && currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is NhienHonChu))
        {
            int count = currentTurn.specialThanThongImpactAfterPlaySkill.Count(x => x is NhienHonChu);
            targetTurn.AddDamageData(new List<int> { count * dameValues[GetIndexLevelThanThong(currentTurn)] });
                

        }


    }
    private void ObserverEffect2(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];
        if (currentTurn == null) return;
        currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);


    }
    public override string GetDesc(int level)
    {
        return $"Nhận phải {GetColoredText($"+{addDebuff}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThanThuong)}. Trong hiệp này, sử dụng thần thông tạo thành thần thức tổn thương thì tăng lên {GetColoredText($"+{dameValues[level - 1]}", "red")} điểm sát thương (Có thể cộng dồn). Kết thúc hiệp giải trừ toàn bộ";

    }
}
