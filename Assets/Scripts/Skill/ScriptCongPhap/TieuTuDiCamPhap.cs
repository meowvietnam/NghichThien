using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TieuTuDiCamPhap))]
public class TieuTuDiCamPhap : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
         {
            20, 25 , 30,35, 40
         };
    [SerializeField]
    private ThanThong thanThongActive;
    public override void ExecuteParams(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        // Kiểm tra caster không null và thỏa mãn điều kiện sử dụng
        if (target != null && IsCanUse(target))
        {
            Execute(caster, target, data);
        }
    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (CombatStatic.IsActiveWithPercent(buffValues[CurrentLevelCongPhapInCombat(target) - 1]))
        {
            Observer.Notify(EventList.Combat.PLAY_SKILL, target, caster, thanThongActive, new List<LinhKhiTieuTan>());

        }
    }

    public override string GetDesc(int level)
    {
        return $"Nhận thần thông tổn thương có {GetColoredText($"{buffValues[level - 1]}%", "red")} tỉ lệ kích hoạt thần thông [Ngự Trận]";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams);

    }
}
