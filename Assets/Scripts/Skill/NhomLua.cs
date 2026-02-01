using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(NhomLua), menuName = "Skill/Thần Thông/" + nameof(NhomLua))]
public class NhomLua : ThanThong
{
    [SerializeField] private int[] dames;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG, ObserverEffect);
    }
    private void ObserverEffect(params object[] data)
    {

        CharacterCombat charTakeDame = (CharacterCombat)data[1];
        CharacterCombat charSendDame = (CharacterCombat)data[0];
        int countSkill = charSendDame.thanThongUseThisComBat.Count(x=>x.thanThong is NhomLua);
        if (CheckIsEqualETypeSkill(charSendDame,new ETypeSkill[] {ETypeSkill.Hoa}))
        {
            charTakeDame.AddDamageData(new System.Collections.Generic.List<int> { countSkill * dames[GetIndexLevelThanThong(charSendDame)] });
        }





    }
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Trong chiến đấu tạo thành {GetTextLinhKhi(ELinhKhi.Hoa)} hệ tổn thương {GetColoredText($"+{dames[level - 1]} điểm", "red")} (Có thể cộng dồn)";
    }

    
}
