using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(SongKich), menuName = "Skill/Thần Thông/" + nameof(SongKich))]

public class SongKich : ThanThong , BaseSkill.LienKich
{
    [SerializeField] protected int[] damePerCountAttack;
    [SerializeField] protected int[] countAttack;
    [SerializeField] protected int[] sucLucs;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        
        List<int> dames = new List<int>();
        for (int i = 0; i < countAttack[GetIndexLevelThanThong(caster) - 1]; i++)
        {
            dames.Add(damePerCountAttack[GetIndexLevelThanThong(caster) - 1]);
        }
        target.TakeDameByThanThong(caster, dames, false);
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ObserverEffect);


    }

    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (currentTurn == null) return;

        int count = currentTurn.specialThanThongImpactAfterPlaySkill
           .Count(x => x is SongKich);
        currentTurn.AddBuffAndDebuffCount(EBuffAndDeBuff.SucLuc, count * sucLucs[GetIndexLevelThanThong(currentTurn)]);
        currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);


    }
    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"{countAttack[level - 1]} lần {damePerCountAttack[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)} Nếu trước đó dùng Kim hệ thần thông, thì thu hoạch được {GetColoredText($"+{sucLucs[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucLuc)} (Có thể cộng dồn)";

    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddThanThongImpactAfterPlaySkill(this);
    }
}
