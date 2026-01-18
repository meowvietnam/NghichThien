using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class TrongNguyenNhuYThe : CongPhap
{
    [SerializeField] private int[] buff;
    [SerializeField] private int[] require;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SucThe, out int sucThe);
        if (sucThe > require[CurrentLevelCongPhapInCombat(caster)-1])
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buff[CurrentLevelCongPhapInCombat(caster) - 1]);
        }    


    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, nếu người có số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucThe)} lớn hơn {GetColoredText($"{require[level - 1]}", "red")} thì thu hoạch được {GetColoredText($"+{buff[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}.";
    }


    public override void Init()
    {
        InitPosCallExecute();
    }

    public override void InitPosCallExecute()
    {
        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, ExecuteParams);

    }
}
