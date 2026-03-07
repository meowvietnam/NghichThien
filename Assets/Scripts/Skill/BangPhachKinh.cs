using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(BangPhachKinh))]
public class BangPhachKinh : CongPhap
{
    [SerializeField]
    private int[] suongGiaValues = new int[]
      {
        2, 3 , 5 ,8, 16
      };
    [SerializeField]
    private int[] noiThuongValues = new int[]
     {
        1, 1 , 2 ,2 , 3
     };
    [SerializeField]
    private int require = 50;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SuongGia, out SecureInt suongGia);
        if(suongGia < require)
        {
            target.AddBuffAndDebuffCount(EBuffAndDeBuff.SuongGia, suongGiaValues[CurrentLevelCongPhapInCombat(caster)-1]);

        } 
        else
        {
            target.AddBuffAndDebuffCount(EBuffAndDeBuff.NoiThuong, noiThuongValues[CurrentLevelCongPhapInCombat(caster)-1]);

        }


    }
    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, nếu đối thủ có số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SuongGia)} thấp hơn {GetColoredText($"{require}", "red")} thì khiến cho đối phương nhận phải {GetColoredText($"+{suongGiaValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SuongGia)}. Nếu không thì, đối phương nhận phải {GetColoredText($"+{noiThuongValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NoiThuong)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, ExecuteParams);

    }
}
