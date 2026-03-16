using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TrongKiemQuyet))]

public class TrongKiemQuyet : CongPhap
{
    [SerializeField] private int[] buff;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        List<ELinhKhi> availableLinhKhi = new List<ELinhKhi>();

        if (caster.khiKim > 0) availableLinhKhi.Add(ELinhKhi.Kim);
        if (caster.khiMoc > 0) availableLinhKhi.Add(ELinhKhi.Moc);
        if (caster.khiThuy > 0) availableLinhKhi.Add(ELinhKhi.Thuy);
        if (caster.khiHoa > 0) availableLinhKhi.Add(ELinhKhi.Hoa);
        if (caster.khiTho > 0) availableLinhKhi.Add(ELinhKhi.Tho);
        if (caster.khiMa > 0) availableLinhKhi.Add(ELinhKhi.Ma);

        // 2. Kiểm tra nếu có ít nhất một loại linh khí để tiêu tán
        if (availableLinhKhi.Count > 0)
        {
            // Chọn ngẫu nhiên một chỉ số trong danh sách
            int randomIndex = UnityEngine.Random.Range(0, availableLinhKhi.Count);
            ELinhKhi selectedLinhKhi = availableLinhKhi[randomIndex];

            // 3. Gọi hàm tiêu tán 1 điểm
            caster.TieuTanLinhKhi(selectedLinhKhi, 1);
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.SucThe, buff[CurrentLevelCongPhapInCombat(caster)-1]);
            // Thêm logic cộng buff Sức Thế ở đây (theo mô tả của hàm GetDesc)
            // Ví dụ: caster.AddBuff(EBuffAndDeBuff.SucThe, buff[level - 1]);
        }


    }
    
    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, ngẫu nhiên tiêu tán 1 điểm linh khí thu hoạch được {GetColoredText($"+{buff[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucThe)}.";
    }


 

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ExecuteParams);

    }
}
