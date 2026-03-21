using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TinhThanThanNiemThe), menuName = "Skill/Thần Thông/" + nameof(TinhThanThanNiemThe))]

public class TinhThanThanNiemThe : ThanThong
{
    [SerializeField]
    private int dameBuffPercent = 15;
    [SerializeField]
    private int takeDameBuffPercent = 30;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
      
        var debuffKeys = caster.buffAndDebuffCount.Keys
                    .Where(key => BaseSkill.eDebuffs.Contains(key))
                    .ToList();
        // 2. Kiểm tra xem có debuff nào để xóa không
        if (debuffKeys.Count > 0)
        {
            for (int i = 0; i < debuffKeys.Count; i++)
            {
                var debuff = debuffKeys[i];
                caster.buffAndDebuffCount.TryGetValue(debuff, out SecureInt debuffValue);
                caster.RemoveBuffAndDebuffCount(debuff, debuffValue);
            }

        }
        caster.AddThanThongImpactAfterPlaySkill(this);
      
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect3);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
        Observer.AddObserver(EventList.Combat.CHARACTER_BEFORE_TAKE_DAME, ObserverEffect2);


    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0]; // người gây dame
        CharacterCombat targetTurn = (CharacterCombat)data[1];// người nhận dame

        if (currentTurn == null) return;

        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is TinhThanThanNiemThe) && CheckIsEqualETypeSkill(currentTurn))
        {
            targetTurn.AddDamageData(new System.Collections.Generic.List<int> { (int)(targetTurn.allDamages[0] * dameBuffPercent/100f) });

        }


    }
    private void ObserverEffect2(params object[] data)
    {
        CharacterCombat targetTurn = (CharacterCombat)data[1];// người nhận dame

        if (targetTurn == null) return;

        if (targetTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is TinhThanThanNiemThe))
        {
            targetTurn.AddDamageData(new System.Collections.Generic.List<int> { (int)(targetTurn.allDamages[0] * takeDameBuffPercent / 100f) });

        }


    }
    private void ObserverEffect3(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (currentTurn == null) return;

        currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);



    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Bản thân gỡ ra toàn bộ trạng thái tiêu cực. Nhận tất cả tổn thương tăng lên {GetColoredText($"{takeDameBuffPercent}%", "red")}. Sử dụng thần thông tạo thành thần thức tổn thương tăng lên {GetColoredText($"{dameBuffPercent}%", "red")}. Sử dụng thần thông này sẽ không giải trừ {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnNac)}";

    }
}
