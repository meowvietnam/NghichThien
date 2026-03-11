using Newtonsoft.Json;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(MaDiemQuyet))]

public class MaDiemQuyet : CongPhap
{
    [SerializeField]
    private int[] thoNguyens = new int[]
      {
        5, 10 , 20 ,40, 80
      };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        return;
    }

    public override string GetDesc(int level)
    {
        return $"Thọ nguyên {GetColoredText($"-{thoNguyens[level-1]}", "red")} năm";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        return;
    }

    public override void EquipCongPhap(CharacterStats statsMax, CharacterStats statsCurrent, int level)
    {
        statsMax.thoNguyen -= thoNguyens[level - 1];
        statsCurrent.thoNguyen -= thoNguyens[level - 1];

    }
    public override void UnEquipCongPhap(CharacterStats statsMax, CharacterStats statsCurrent, int level)
    {
        statsMax.thoNguyen += thoNguyens[level - 1];
        statsCurrent.thoNguyen += thoNguyens[level - 1];


    }
}
