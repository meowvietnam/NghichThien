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

    public override void EquipCongPhap(CharacterStats statsCurrent, int daiCanhGioiCongPhap)
    {

        statsCurrent.thoNguyen -= thoNguyens[daiCanhGioiCongPhap - 1];

    }
    public override void UnEquipCongPhap(CharacterStats statsCurrent, int daiCanhGioiCongPhap)
    {

        statsCurrent.thoNguyen += thoNguyens[daiCanhGioiCongPhap - 1];


    }
}
