using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(VanKhiThuat), menuName = "Skill/Thần Thông/" + nameof(VanKhiThuat))]

public class VanKhiThuat : ThanThong
{
   
    [SerializeField]
    private int hapThuSO = 3;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int hapThu = this.hapThuSO;
        TuDuongHonNguyenKinh tuDuongHonNguyenKinh = caster.congPhapEquip.FirstOrDefault(x => x.congPhap is TuDuongHonNguyenKinh).congPhap as TuDuongHonNguyenKinh;
        if(tuDuongHonNguyenKinh != null)
        {
            if (CombatStatic.IsActiveWithPercent(tuDuongHonNguyenKinh.GetPercent(caster)))
            {
                hapThu *= 2;
            }    
        }    
        
       
        HauKeVoLuc(caster);
        caster.HapThuLinhKhi(hapThu);

    }
  
   
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Hấp thụ {GetColoredText($"{hapThuSO}", "red")} điểm linh khí";

    }
}
