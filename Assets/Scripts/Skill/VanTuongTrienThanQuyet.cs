using LongDH.LongDHModules;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(VanTuongTrienThanQuyet))]
public class VanTuongTrienThanQuyet : CongPhap
{
    [SerializeField] private int[] buffValues;
    [SerializeField] private int[] requireActive;





    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        int require = requireActive[CurrentLevelCongPhapInCombat(caster) - 1];
        int trienNhieu = buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
        int dame = target.allDamages[0];
        if (CheckIsEqualETypeSkill(caster)&&dame >= require)
        {
            target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu, trienNhieu);

        }



    }

    public override string GetDesc(int level)
    {
        return $"Nếu sử dụng {GetTextTypeSkill()} hệ thần thông gây {GetColoredText($"{requireActive[level - 1]} điểm", "red")} tổn thương thì đối phương nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)}";

    }
    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG, ExecuteParams);

    }

}
