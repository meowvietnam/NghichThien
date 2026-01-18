using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class TrongNguyenVoPhongCong : CongPhap
{
    [SerializeField] private int[] buff;
    [SerializeField] private int require = 2;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ThanThong thanThongUse = (ThanThong)data[2];
        if(thanThongUse is SucThe && caster.khiKim >= require)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.SucThe, buff[CurrentLevelCongPhapInCombat(caster) - 1]);
            caster.TieuTanLinhKhi(ELinhKhi.Kim,require);

        }

    }

    public override string GetDesc(int level)
    {
        return $"Sử dụng thần thông {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucThe)} thì sẽ tiêu hao thêm {GetColoredText($"{require} điểm kim", "red")} linh khí thì thu hoạch được {GetColoredText($"+{buff[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucThe)}";
    }

    public override void Init()
    {
        InitPosCallExecute();
    }

    public override void InitPosCallExecute()
    {
        Observer.AddObserver(EventList.GamePlay.PLAY_SKILL, ExecuteParams);

    }
}
