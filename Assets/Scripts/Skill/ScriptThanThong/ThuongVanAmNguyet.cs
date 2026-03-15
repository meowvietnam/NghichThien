using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
[CreateAssetMenu(fileName = nameof(ThuongVanAmNguyet), menuName = "Skill/Thần Thông/" + nameof(ThuongVanAmNguyet))]

public class ThuongVanAmNguyet : ThanThong
{
    [SerializeField]
    private int dameBuffPercent = 20;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        
        caster.AddThanThongImpactAfterPlaySkill(this);

        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ObserverEffect3);
        Observer.AddObserver(EventList.GamePlay.CACULATE_DAME_BY_THANTHONG, ObserverEffect);


    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0]; // người gây dame
        CharacterCombat targetTurn = (CharacterCombat)data[1];// người nhận dame

        if (targetTurn == null) return;

        if (targetTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is ThuongVanAmNguyet))
        {
            List<int> dames = new 
                List<int>();
            for (int i = 0; i < targetTurn.allDamages.Count; i++)
            {
                int item = targetTurn.allDamages[i];
                int dame = (int)(item * dameBuffPercent / 100f);
                targetTurn.allDamages[i] -= dame;
                dames.Add(dame);
            }
            currentTurn.TakeDameByStatic(targetTurn, dames, true, eTypeSkill);

           
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
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Khí nhận phải tổn thương từ thần thông thì giảm {GetColoredText($"{dameBuffPercent}%", "red")} sát thương. Đồng thời đem lượng sát thương đó phản lại đối thủ";

    }
}
