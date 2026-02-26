using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(VanKhiThuat), menuName = "Skill/Thần Thông/" + nameof(VanKhiThuat))]

public class VanKhiThuat : ThanThong
{
   
    [SerializeField]
    private int hapThu = 3;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.HapThuLinhKhi(hapThu);

    }
  
   
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Hấp thụ {GetColoredText($"{hapThu}", "red")} điểm linh khí";

    }
}
