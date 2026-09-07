using LongDH.LongDHModules;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HuyenVuChanPhap))]
public class HuyenVuChanPhap : CongPhap, IMemorySerializable<HuyenVuChanPhap>
{
    public override void ExecuteParams(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        // Tìm trong danh sách trang bị của caster
        if (target != null && IsCanUse(target))
        {
            Execute(caster, target, data);
        }
    }

    [SerializeField] private int[] percentActives;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // target là ng né thành công
        // caster là ng gây dame
        if (!CombatStatic.IsActiveWithPercent(percentActives[CurrentLevelCongPhapInCombat(target) - 1]))
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
        return
            $"Mỗi lần né tránh thành công, có {GetColoredText($"{percentActives[level - 1]}%", "red")} tỉ lệ đánh cắp một điểm linh khí bất kỳ của đối thủ";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NE_TRANH_SUCCESS, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HuyenVuChanPhap value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}