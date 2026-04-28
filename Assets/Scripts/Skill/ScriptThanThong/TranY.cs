using UnityEngine;
[CreateAssetMenu(fileName = nameof(TranY), menuName = "Skill/Thần Thông/" + nameof(TranY))]

public class TranY : ThanThong
{
    

    [SerializeField]
    private int[] buffValues = new int[]
    {
        1,2,3,4,6
    };
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int giamTonThuong = buffValues[GetIndexLevelThanThong(caster)];


        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.GiamTonThuong, giamTonThuong);

        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Tran, out SecureInt  tran);


        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.GiamTonThuong, tran);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Tran, tran);
        ShowSkillEffectWorld(caster, delayTimeEffect);

    }



    public override string GetDesc(int level)
    {
        return $"Thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.GiamTonThuong)}, Bản thân gỡ ra bao nhiêu tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)} thì thu hoạch được bấy nhiêu {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";

    }
}
