using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(DanDocChu), menuName = "Skill/Thần Thông/" + nameof(DanDocChu))]

public class DanDocChu : ThanThong
{


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        CombatStatic.TrungDoc(caster,target);


    }

    public override List<RequireActive> GetRequireActive(CharacterCombat caster)
    {
        List<RequireActive> listRqA = new List<RequireActive>(requireActives);
        foreach (RequireActive requireActive in requireActives)
        {
            if(requireActive.linhKhiRequire==ELinhKhi.Moc)
            {
                requireActive.count = 1 + caster.thanThongUseThisTurn.Count(x=>x.thanThong is DanDocChu);
                break;
            }
        }

        return listRqA;
    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        int countTrungDoc = 0;
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrungDoc, out countTrungDoc);
        return countTrungDoc > 0;
    }



    public override string GetDesc(int level)
    {
        return $"Đối phương bị kích phát hiệu quả {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)}. Trong hiệp này, mỗi một lần sử dụng thần thông này thì tiêu hao thêm một điểm {GetTextLinhKhi(ELinhKhi.Moc)} linh khí";
    }
}
