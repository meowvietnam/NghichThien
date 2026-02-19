using UnityEngine;
using LongDH.LongDHModules;
using Newtonsoft.Json;
using System.Linq;
[CreateAssetMenu(fileName = nameof(PhanHoaThoDiemQuyet), menuName = "Skill/Thần Thông/" + nameof(PhanHoaThoDiemQuyet))]

public class PhanHoaThoDiemQuyet : ThanThong
{
    [SerializeField]
    private int[] dameValues = new int[]
       {
        6,9,13,24,44
       };
    [SerializeField]
    private int[] dameValueBuffs = new int[]
    {
        2,2,3,3,5
    };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dameValues[GetIndexLevelThanThong(caster)];
        int tongSoLuongHoa = caster.thanThongUseThisTurn
    .SelectMany(x => x.linhKhiTieuTan) // Gộp tất cả linh khí lại
    .Where(z => z.eLinhKhi == ELinhKhi.Hoa) // Chỉ lọc ra những cái hệ Hỏa
    .Sum(z => z.count); // Cộng dồn tất cả giá trị của thuộc tính Count lại
        int dameBuff = dameValueBuffs[GetIndexLevelThanThong(caster)] * tongSoLuongHoa;
        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame+dameBuff });



    }



    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dameValues[level - 1]}", "red")} điểm sát thương. Trong hiệp này, mỗi sử dụng qua một điểm {GetTextLinhKhi(ELinhKhi.Hoa)} hệ linh khí thì tăng lên {GetColoredText($"+{dameValueBuffs[level - 1]}", "red")} điểm sát thương ";

    }
}
