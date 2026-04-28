using Skill;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(VanThuyKiemDien))]

public class VanThuyKiemDien : CongPhap
{

    [SerializeField]
    private int[] buffValues = new int[]
    {
        2, 3 , 4 ,6 , 12
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        // CH? REMOVE KHI ?Ã TH?C THI BUFF THÀNH CÔNG



    }
    public override string GetDesc(int level)
    {
        return $"Bắt đầu chiến đấu, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KinhDao)} và {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChiThuy)}";

    }




    public override void InitPosCallExecute(CharacterCombat caster)
    {
        RemoveCongPhap(caster);
        int buff = buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KinhDao, buff);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChiThuy, buff);

    }
}
