using UnityEngine;
using LongDH.LongDHModules;
using Newtonsoft.Json;
using System.Linq;
using Skill;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(PhanHoaThoDiemQuyet), menuName = "Skill/Thần Thông/" + nameof(PhanHoaThoDiemQuyet))]
public class PhanHoaThoDiemQuyet : ThanThong, IMemorySerializable<PhanHoaThoDiemQuyet>
{
    [SerializeField]
    private int[] dameValues = new int[]{6, 9, 13, 24, 44};
    [SerializeField]
    private int[] dameValueBuffs = new int[]{2, 2, 3, 3, 5};
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dameValues[GetIndexLevelThanThong(caster)];
        int tongSoLuongHoa = caster.thanThongUseThisTurn.SelectMany(x => x.linhKhiTieuTan) // Gộp tất cả linh khí lại
        .Where(z => z.eLinhKhi == ELinhKhi.Hoa) // Chỉ lọc ra những cái hệ Hỏa
        .Sum(z => z.count); // Cộng dồn tất cả giá trị của thuộc tính Count lại
        int dameBuff = dameValueBuffs[GetIndexLevelThanThong(caster)] * tongSoLuongHoa;
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dame + dameBuff}, eTypeSkill);
        PlayVisualSkill(caster, target);
    }
    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dameValues[level - 1]}", "red")} điểm sát thương. Trong hiệp này, mỗi sử dụng qua một điểm {GetTextLinhKhi(ELinhKhi.Hoa)} hệ linh khí thì tăng lên {GetColoredText($"+{dameValueBuffs[level - 1]}", "red")} điểm sát thương ";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref PhanHoaThoDiemQuyet value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}