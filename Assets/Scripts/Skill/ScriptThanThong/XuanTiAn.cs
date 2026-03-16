using System.Linq;
using UnityEngine;
using LongDH.LongDHModules;

[CreateAssetMenu(fileName = nameof(XuanTiAn), menuName = "Skill/Thần Thông/" + nameof(XuanTiAn))]

public class XuanTiAn : ThanThong
{
    [SerializeField] private int[] dames;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect2);
    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        if (currentTurn == null) return;

        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is XuanTiAn))
        {
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);

        }


    }
    private void ObserverEffect2(params object[] data)
    {

        CharacterCombat charSendDame = (CharacterCombat)data[0];
        CharacterCombat charTakeDame = (CharacterCombat)data[1];

        if (charSendDame == null) return;
        if (charSendDame.specialThanThongImpactAfterPlaySkill.Any(x => x is XuanTiAn) && CheckIsEqualETypeSkill(charSendDame,new ETypeSkill[] {ETypeSkill.Moc}) )
        {
            charTakeDame.AddDamageData(new System.Collections.Generic.List<int> { dames[GetIndexLevelThanThong(charSendDame) ]});
            charSendDame.RemoveAllThanThongImpactAfterPlaySkill(this);

        }



    }
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Lần tiếp theo sửa dụng thần thông tạo thành {GetTextLinhKhi(ELinhKhi.Moc)} hệ tổn thương thì sát thương {GetColoredText($"+{dames[level - 1]}", "red")}";
    }


}
