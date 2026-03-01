using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TuLinhTran), menuName = "Skill/Thần Thông/" + nameof(TuLinhTran))]

public class TuLinhTran : TranPhapTT
{
    [SerializeField] private int[] hapThus;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        ExecuteTranPhap(caster,target);


    }
    public override void ObserverEffect(params object[] data)
    {
        return;
    }

    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        caster.totalKhiRandom += hapThus[GetIndexLevelThanThong(caster)];

    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Giai đoạn hấp thụ linh khí {GetColoredText($"{hapThus[level-1]}", "red")}";
    }

   
}
