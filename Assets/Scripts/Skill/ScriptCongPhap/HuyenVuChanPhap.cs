using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HuyenVuChanPhap))]
public class HuyenVuChanPhap : CongPhap
{
    public override void ExecuteParams(params object[] data)
    {

        CharacterCombat caster = (CharacterCombat)data[0];

        CharacterCombat target = (CharacterCombat)data[1];
        // Tìm trong danh sách trang bị của caster
        if (IsCanUse(target))
        {

            Execute(caster, target, data);

        }
    }
    [SerializeField] private int[] percentActives;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // target là ng né thành công
        // caster là ng gây dame
        if (!CombatStatic.IsActiveWithPercent(
           percentActives[CurrentLevelCongPhapInCombat(target)-1]))
            return;

        // Lấy linh khí hợp lệ
        ELinhKhi linhKhi = caster.GetRandomLinhKhiConLai();

        if (linhKhi == ELinhKhi.None)
            return;

        int count = 1; // hoặc random / scale theo skill
        caster.TieuTanLinhKhi(linhKhi, count);
        // target hút linh khí
        target.HapThuLinhKhi(linhKhi, count);
    }

    public override string GetDesc(int level)
    {
        return $"Mỗi lần né tránh thành công, có {GetColoredText($"{percentActives[level-1]}%", "red")} tỉ lệ đánh cắp một điểm linh khí bất kỳ của đối thủ";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.NE_TRANH_SUCCESS, ExecuteParams);
    }


}
