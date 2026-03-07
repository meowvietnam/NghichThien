using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(NhieuHonThuat), menuName = "Skill/Thần Thông/" + nameof(NhieuHonThuat))]
public class NhieuHonThuat : ThanThong, BaseSkill.ThanThuc
{
    [SerializeField]
    private int[] dames = new int[]
   {
        3,4,5,8,16
   };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame });
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ObserverEffect);

    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (targetTurn == null) return;

        if (targetTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is NhieuHonThuat))
        {
            int countRemove = currentTurn.CurrentLinhKhi - currentTurn.totalKhi;
            for (int i = 0; i < (int)(countRemove/2); i++)
            {
                currentTurn.TieuTanLinhKhi(currentTurn.GetRandomLinhKhiConLai(),1);
            }
            targetTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
        }


    }
    public void ExecuteThanThuc(CharacterCombat caster, CharacterCombat target)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, true);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.ThanThuc)} thì tổn thương gấp đôi. Đối thủ hiệp sau, hấp thụ số lượng linh khí giảm một nửa";

    }
}
