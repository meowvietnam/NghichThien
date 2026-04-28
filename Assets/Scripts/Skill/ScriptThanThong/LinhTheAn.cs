using LongDH.LongDHModules;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
[CreateAssetMenu(fileName = nameof(LinhTheAn), menuName = "Skill/Thần Thông/" + nameof(LinhTheAn))]
public class LinhTheAn : ThanThong
{
    [SerializeField] private int[] hoTraos;
    [SerializeField] private int[] hoTraoAdds;
    [SerializeField] private int require = 1;
    [SerializeField] private float delayTimeEffect = 0.5f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddThanThongImpactAfterPlaySkill(this);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hoTraos[GetIndexLevelThanThong(caster)]);
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ObserverEffect);
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect2);
        ShowSkillEffectWorld(caster, delayTimeEffect);


    }
    private void ObserverEffect(params object[] data)
    {
        Debug.Log("có gọi vào LinhQuangANRemoveCheck1");
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        ThanThong thanThongUse = (ThanThong)data[2];
       
        if (currentTurn == null) return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is LinhTheAn))
        {

            List<LinhTheAn> listThisSkill = currentTurn.specialThanThongImpactAfterPlaySkill
                                 .OfType<LinhTheAn>()
                                 .ToList();
            int requireNew = require*listThisSkill.Count;
            int hoTraoAdd = hoTraoAdds[GetIndexLevelThanThong(currentTurn)] * listThisSkill.Count;

            if (CheckIsEqualETypeSkill(currentTurn) && currentTurn.khiKim >= requireNew)
            {
                currentTurn.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hoTraoAdd);
                currentTurn.TieuTanLinhKhi(ELinhKhi.Kim, requireNew);

            }

        }



    }
    private void ObserverEffect2(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        if (currentTurn == null) return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is LinhTheAn))
        {
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);

        }




    }
    public override string GetDesc(int level)
    {
        return $"Thu hoạch được {GetColoredText($"+{hoTraos[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}. Trong hiệp này, nếu sử dụng {GetTextLinhKhi(ELinhKhi.Kim)} hệ thần thông, tiêu hao {GetColoredText($"X điểm", "red")} {GetTextLinhKhi(ELinhKhi.Kim)} linh khí sẽ thu hoạch được {GetColoredText($"+{hoTraoAdds[level - 1]}*X", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} (X là số lần sử dụng thần thông này trong hiệp)";
    }
}
