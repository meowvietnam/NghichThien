using UnityEngine;
[CreateAssetMenu(fileName = nameof(VanTranHoaTho), menuName = "Skill/Thần Thông/" + nameof(VanTranHoaTho))]
public class VanTranHoaTho : ThanThong
{
    [SerializeField]
    private int[] buffValues = new int[]
    {
        1,1,1,1,1
    };
    [SerializeField] protected float delayTimeEffect = 1;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Tran, out SecureInt  tran);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanTho, tran * buffValues[GetIndexLevelThanThong(caster)]);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Tran, tran);
        ShowSkillEffectWorld(caster, delayTimeEffect);

    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Bản thân gỡ ra toàn bộ {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)}, mỗi một tầng gỡ ra thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanTho)}";

    }
}
