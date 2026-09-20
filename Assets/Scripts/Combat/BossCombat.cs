using LongDHModules;
using UnityEngine;
using System.Linq;
public class BossCombat : CharacterCombat
{
    public int IdSkill => 526;
    public int LimitDame => 1000000;
    public override void Init(GameManager.CharCombatInit charCombatInit)
    {
        base.Init(charCombatInit);
        Pool.Get<GameManager>().GetThanThongWithId(IdSkill).LoadFirstDataPrefab();
    }
    public void BossPlayDame()
    {
        var thanThong = Pool.Get<GameManager>().GetThanThongWithId(IdSkill);
        VisualPlaySkill(thanThong);
        thanThong.PlayVisualSkill(this, Pool.Get<CombatManager>().char1);


    }
    protected override void TakeDame(CharacterCombat sender, bool isDef)
    {
        // 1. Kiểm tra tổng sát thương bằng long để tránh overflow
        if (allDamages.Select(d => (long)d).Sum() >= LimitDame)
        {
            int count = allDamages.Count;

            if (count > 0)
            {
                // Tính lượng dame trung bình cho mỗi phần tử
                int baseDame = LimitDame / count;
                int remainder = LimitDame % count; // Phần dư (nếu có)

                for (int i = 0; i < count; i++)
                {
                    allDamages[i] = baseDame;
                }

                // Cộng phần dư vào phần tử cuối để tổng đúng chính xác bằng LimitDame
                allDamages[count - 1] += remainder;
            }
        }
        base.TakeDame(sender, isDef);
    }
}
