using UnityEngine;

[CreateAssetMenu(fileName = nameof(HapThuLinhKhi), menuName = "Đan dược/Đan dược Combat/" + nameof(HapThuLinhKhi))]
public class HapThuLinhKhi : BaseDanDuocCombat
{
    [SerializeField] private int hapThu = 1;

    public override void ExecuteDanDuoc(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        caster.HapThuLinhKhi(hapThu * GetDanDuocCombat(caster).count);
    }
}