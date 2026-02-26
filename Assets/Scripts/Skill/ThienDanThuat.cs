using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThienDanThuat), menuName = "Skill/Thần Thông/" + nameof(ThienDanThuat))]
public class ThienDanThuat : ThanThong
{
    [SerializeField]
    private int[] buffValues = new int[]
      {
        12,18,28,50,100
      };

   
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG, ObserverEffect);

    }
    private void ObserverEffect(params object[] data)
    {

        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (currentTurn == null) return;

        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is ThienDanThuat) && CheckIsEqualETypeSkill(currentTurn))
        {
            int count = currentTurn.specialThanThongImpactAfterPlaySkill.Count(x => x is ThienDanThuat);
            targetTurn.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DanLuc, out int danLuc);
            targetTurn.AddDamageData(new System.Collections.Generic.List<int> { buffValues[GetIndexLevelThanThong(currentTurn)] * danLuc * count });
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);

        }


    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Lần tiếp theo sử dụng {GetTextLinhKhi(ELinhKhi.Tho)} hệ thần thông gây tổn thương. Đối thủ mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DanLuc)} thì tổn thương {GetColoredText($"+{buffValues[level - 1]}", "red")} (Có thể cộng dồn)";

    }
}
