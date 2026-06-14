using UnityEngine;
using Skill;
[CreateAssetMenu(menuName = "Skill/Độn Thuật/" + nameof(NguKiemPhiHanh))]
public class NguKiemPhiHanh : CongPhap
{
    [SerializeField]
    private int[] buffValues;
    public override void EquipCongPhap(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
    {
        statsCurrent.tocDoBay += buffValues[daiCanhGioiCongPhap - 1];
    }
    public override void UnEquipCongPhap(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
    {
        statsCurrent.tocDoBay -= buffValues[daiCanhGioiCongPhap - 1];

    }
}
