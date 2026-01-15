using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class ToKimChanPhap : CongPhap
{
    [SerializeField] private int[] dame;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        List<LinhKhiTieuTan> linhKhiTieuTan = data[2] as List<LinhKhiTieuTan>;
        int count = 0;
        foreach (var item in linhKhiTieuTan)
        {
            count += item.count;
        }
        Debug.Log("lmm"+count);
        target.TakeDameCaculate(caster,new List<int> { count * dame[CurrentLevelCongPhapInCombat(caster)-1]});
        target.TakeDame();
    }

    public override string GetDesc(int level)
    {
        return $"Giai đoạn tiêu tán linh khí, mỗi tiêu tán 1 điểm linh khí đối phương nhận {GetColoredText($"+{dame[level - 1]}", "red")} sát thương";
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
   
