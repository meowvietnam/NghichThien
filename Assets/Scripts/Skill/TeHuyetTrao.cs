using UnityEngine;
[CreateAssetMenu(fileName = nameof(TeHuyetTrao), menuName = "Skill/Thần Thông/" + nameof(TeHuyetTrao))]

public class TeHuyetTrao : ThanThong
{
    [SerializeField]
    private int[] buffValues = new int[]
   {
        20,30,50,80,160
   };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        int hp = buffValues[GetIndexLevelThanThong(caster)];

        caster.AddDamageData(new System.Collections.Generic.List<int> { hp});
        caster.TakeDame(null);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hp);

    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Chuyển hóa {GetColoredText($"{buffValues[level - 1]}", "red")} điểm HP của bản thân thành {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";

    }
}
