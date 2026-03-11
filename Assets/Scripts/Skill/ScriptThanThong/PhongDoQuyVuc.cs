using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(PhongDoQuyVuc), menuName = "Skill/Thần Thông/" + nameof(PhongDoQuyVuc))]
public class PhongDoQuyVuc : TranPhapTT
{
    [SerializeField]
    private int[] buffValues = new int[]
     {
        1,1,1,1,1
     };
    [SerializeField]
    private int[] buffValues2 = new int[]
    {
        0,1,1,2,3
    };
    [SerializeField]
    private int[] buffValues3 = new int[]
    {
        4,0,8,10,16
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, ObserverEffect);


    }
    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.AnhNac, buffValues[GetIndexLevelThanThong(caster)]);

        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.AnhDich, buffValues2[GetIndexLevelThanThong(caster)]);

        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Anh, buffValues3[GetIndexLevelThanThong(caster)]);

    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Kết thúc hiệp, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnhNac)}, {GetColoredText($"+{buffValues2[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnhDich)} và {GetColoredText($"+{buffValues3[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Anh)}";
    }

}
