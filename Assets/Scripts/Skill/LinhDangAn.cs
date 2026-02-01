using System.Linq;
using UnityEngine;
using LongDH.LongDHModules;

[CreateAssetMenu(fileName = nameof(LinhDangAn), menuName = "Skill/Thần Thông/" + nameof(LinhDangAn))]

public class LinhDangAn : ThanThong
{
    [SerializeField] private int[] trienNhieu;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        Observer.AddObserver(EventList.GamePlay.LIEN_KICH, ObserverEffect);



    }

    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (currentTurn == null) return;

        int count = currentTurn.thanThongUseThisComBat.Count(x => x.thanThong is LinhDangAn);
        targetTurn.AddBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu, count * trienNhieu[GetIndexLevelThanThong(currentTurn)]);



    }
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Trong chiến đấu, mỗi lần phát động ngũ hành {GetKeyWord(EKeyWord.LienKich)} thì đối phương nhận phải {GetColoredText($"+{trienNhieu[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)} (Thần thông này có thể cộng dồn)";

    }
}
