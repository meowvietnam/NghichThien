using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThuongVanAmNguyet), menuName = "Skill/Thần Thông/" + nameof(ThuongVanAmNguyet))]

public class ThuongVanAmNguyet : ThanThong
{
    [SerializeField]
    private int[] dameBuffPercents = new int[]
    {
       20 , 25 , 30 ,35,40
    };
    [SerializeField] private float cameraSize = 5;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);

        caster.AddThanThongImpactAfterPlaySkill(this);

        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect3);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
        caster.effectSkillNoDestroy.Add(this, ShowSkillEffectParticleBase(caster, 0, cameraSize, false));


    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0]; // người gây dame
        CharacterCombat targetTurn = (CharacterCombat)data[1];// người nhận dame

        if (targetTurn == null) return;

        if (targetTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is ThuongVanAmNguyet))
        {
            int dameBuffPercent = dameBuffPercents[GetIndexLevelThanThong(targetTurn)];
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
            targetTurn.effectSkillNoDestroy.TryGetValue(this, out var effect);
            if (effect != null)
            {
                var effectAttack = effect.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0);
                effectAttack.gameObject.SetActive(false);
                effectAttack.gameObject.SetActive(true);

            }

        }


    }

    private void ObserverEffect3(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (currentTurn == null) return;

        currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
        currentTurn.RemoveEffectSkillNoDestroy(this);


    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Khi nhận phải tổn thương từ thần thông thì giảm {GetColoredText($"{dameBuffPercents[level - 1]}%", "red")} sát thương. Đồng thời đem lượng sát thương đó phản lại đối thủ";

    }
}
