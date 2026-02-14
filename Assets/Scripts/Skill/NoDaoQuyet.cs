using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(NoDaoQuyet))]
public class NoDaoQuyet : CongPhap
{
    [SerializeField] private ThanThong thanThongActive;
    [SerializeField] private int[] percentActive;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(CheckIsEqualETypeSkill(caster) && CombatStatic.IsActiveWithPercent(percentActive[CurrentLevelCongPhapInCombat(caster)-1]))
        {
            Observer.Notify(EventList.GamePlay.PLAY_SKILL, caster, target, thanThongActive, new List<LinhKhiTieuTan>());

        }
    }

    public override string GetDesc(int level)
    {
        return $"Sử dụng {GetTextTypeSkill()} hệ thần thông gây tổn thương, có {GetColoredText($"{percentActive[level - 1]}%", "red")} tỷ lệ phóng thích một lần {GetColoredText("[Thủy Thương]", "red")} lên đối phương";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG, ExecuteParams);

    }


}
