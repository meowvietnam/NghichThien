using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(DoatLinhThuat), menuName = "Skill/Thần Thông/" + nameof(DoatLinhThuat))]

public class DoatLinhThuat : ThanThong
{
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.HapThuLinhKhi(ELinhKhi.Kim,target.khiKim);
        caster.HapThuLinhKhi(ELinhKhi.Moc, target.khiMoc);
        caster.HapThuLinhKhi(ELinhKhi.Thuy, target.khiThuy);
        caster.HapThuLinhKhi(ELinhKhi.Hoa, target.khiHoa);
        caster.HapThuLinhKhi(ELinhKhi.Tho, target.khiTho);
        caster.HapThuLinhKhi(ELinhKhi.Ma, target.khiMa);

        target.TieuTanAllLinhKhi();

    }


    public override string GetDesc(int level)
    {
        return $"Đánh cắp toàn bộ linh khí của đối thủ";

    }
}
