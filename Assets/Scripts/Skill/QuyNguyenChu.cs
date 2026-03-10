using LongDH.LongDHModules;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(QuyNguyenChu), menuName = "Skill/Thần Thông/" + nameof(QuyNguyenChu))]

public class QuyNguyenChu : ThanThong
{
    [SerializeField] private int hapThu = 5;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, ObserverEffect);

    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (currentTurn == null) return;

        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is QuyNguyenChu))
        {
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
            currentTurn.HapThuLinhKhi(hapThu);

        }


    }


    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Kết thúc hiệp, hấp thụ {GetColoredText($"{hapThu}", "red")} điểm linh khí";

    }
}
