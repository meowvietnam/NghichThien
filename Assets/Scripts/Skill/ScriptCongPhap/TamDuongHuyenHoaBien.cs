using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TamDuongHuyenHoaBien))]

public class TamDuongHuyenHoaBien : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
      {
        12, 14 , 16,18, 20
      };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        List<int> dames = (List<int>)data[2];
        int dame = dames[0];
        if (CheckIsEqualETypeSkill(caster))
        {
            // 1. Lấy danh sách các Thần Thông hệ DiHoa duy nhất (không trùng Object)
            var uniqueDiHoaSkills = caster.thanThongUseThisComBat
                .SelectMany(innerList => innerList)           // Gộp tất cả List con thành 1 danh sách phẳng
                .Where(data => data.thanThong != null)        // Đảm bảo object thanThong không null
                .Select(data => data.thanThong)               // Lấy object ThanThong ra
                .Where(tt => tt.eTypeSkill.Contains(ETypeSkill.DiHoa)) // Kiểm tra nếu có hệ DiHoa
                .Distinct()                                   // Lọc bỏ các Object trùng nhau
                .ToList();

            // 2. Lấy số lượng để tính toán buff
            int countUniqueDiHoa = uniqueDiHoaSkills.Count;

            // 3. Tính toán sát thương tăng thêm
            float totalBuff = (countUniqueDiHoa * buffValues[CurrentLevelCongPhapInCombat(caster) - 1])/100f;
           
            target.AddDamageData(new System.Collections.Generic.List<int> { (int)(totalBuff*dame) });
        }



    }

    public override string GetDesc(int level)
    {
        return $"Trong trận chiến, mỗi sử dụng qua một loại thần thông {GetColoredText($"[{GetTextTypeSkill(ETypeSkill.DiHoa)}]", "red")} khác nhau thì {GetTextLinhKhi(ELinhKhi.Hoa)} hệ thần thông sát thương tăng lên {GetColoredText($"{buffValues[level - 1]}%", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams);

    }
}
