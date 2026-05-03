using UnityEngine;
[CreateAssetMenu(fileName = nameof(TeHuyetTrao), menuName = "Skill/Thần Thông/" + nameof(TeHuyetTrao))]

public class TeHuyetTrao : ThanThong
{
    [SerializeField]
    private int[] buffValues = new int[]
   {
        20,30,50,80,160
   };
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        int hp = buffValues[GetIndexLevelThanThong(caster)];

        caster.TakeDameByStatic(null, new System.Collections.Generic.List<int> { hp }, false, null, false);

        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hp);
        ShowSkillEffectWorld(caster, delayTimeEffect);


    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Chuyển hóa {GetColoredText($"{buffValues[level - 1]}", "red")} điểm HP của bản thân thành {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";

    }
}
