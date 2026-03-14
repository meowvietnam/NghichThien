using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(NhienTan), menuName = "Skill/Thần Thông/" + nameof(NhienTan))]

public class NhienTan : ThanThong
{
    [SerializeField]
    private int[] hapThu = new int[]
      {
        5,5,5,5,5
      };
    [SerializeField]
    private int[] requires = new int[]
      {
        3,3,3,3,3
      };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Diem, requires[GetIndexLevelThanThong(caster)]);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, requires[GetIndexLevelThanThong(caster)]);

        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ObserverEffect);






    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (currentTurn == null) return;

        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is BangVuThuat))
        {
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
            currentTurn.HapThuLinhKhi(ELinhKhi.Hoa, hapThu[GetIndexLevelThanThong(currentTurn)]);

        }


    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Diem, out SecureInt  diem);
        if (diem >= requires[GetIndexLevelThanThong(caster)] && base.IsCanUse(caster, target))
        {
            return true;
        }
        return false;
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Bản thân gỡ ra {GetColoredText($"{requires[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Diem)} và nhận lại {GetColoredText($"+{requires[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}. Bắt đầu hiệp, hấp thụ {GetColoredText($"+{hapThu[level - 1]}", "red")} điểm {GetTextLinhKhi(ELinhKhi.Hoa)} hệ linh khí";

    }
}
