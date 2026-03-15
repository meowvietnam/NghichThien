using UnityEngine;

public abstract class CPTieuHaoThoNguyen : CongPhap
{
    [SerializeField]
    private int[] tieuHao = new int[]
    {
        2,4,6,8,10
    };
    public override void UpgradeCongPhapSuccess(CharacterStats statsCurrent, int daiCanhGioiCongPhap)
    {
        base.UpgradeCongPhapSuccess(statsCurrent, daiCanhGioiCongPhap);
        statsCurrent.thoNguyen -= tieuHao[daiCanhGioiCongPhap - 1];
    }
    public override void LearnCongPhapSuccess(CharacterStats statsCurrent, int daiCanhGioiCongPhap)
    {
        base.LearnCongPhapSuccess(statsCurrent, daiCanhGioiCongPhap);
        statsCurrent.thoNguyen -= tieuHao[daiCanhGioiCongPhap - 1];


    }
    public override string GetDesc(int level)
    {
        return $"(Học tập hoặc đột phá tiêu hao thọ nguyên {GetColoredText($"{tieuHao[level-1]} năm", "red")})";
    }
}
