using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(SaPhuoc), menuName = "Skill/Thần Thông/" + nameof(SaPhuoc))]

public class SaPhuoc : ThanThong
{
    [SerializeField] private HoanTran hoanTran;
    [SerializeField]
    private int[] dames = new int[]
    {
        4,5,9,16,32
    };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        caster.AddThanThongCoolDown(hoanTran, -1);
        Observer.Notify(EventList.Combat.PLAY_SKILL, caster, target, hoanTran, new List<LinhKhiTieuTan>());
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Cat, out SecureInt  cat);
        if (cat > 0 && base.IsCanUse(caster, target))
        {
            return true;
        }
        return false;
    }

    public override List<RequireActive> GetRequireActive(CharacterCombat caster)
    {
        List<RequireActive> listRqA = new List<RequireActive>();

        foreach (var requireActive in requireActives)
        {
            // clone object
            RequireActive clone = new RequireActive()
            {
                linhKhiRequire = requireActive.linhKhiRequire,
                count = requireActive.count
            };

            if (clone.linhKhiRequire == ELinhKhi.Tho)
            {
                int extra = caster.thanThongUseThisTurn
                    .Count(x => x.thanThong is SaPhuoc);

                clone.count += extra;
            }

            listRqA.Add(clone);
        }

        return listRqA;
    }
    public override string GetDesc(int level)
    {
        return $"Đối thủ có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Cat)} mới có thể sử dụng. Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Đồng thời, kích hoạt thần thông {GetColoredText($"[Hoàn Trần]", "red")}. Trong hiệp này, mỗi một lần sử dụng thần thông này thì tiêu hao thêm một điểm {GetTextLinhKhi(ELinhKhi.Tho)} linh khí";

    }
}
