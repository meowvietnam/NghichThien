using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HonNguyenTranDanLuc))]
public class HonNguyenTranDanLuc : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
         {
            4, 5 , 6,9, 15
         };
  
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DanLuc, out SecureInt  danLuc);
        if(danLuc > 0)
        {
            ThanThong thanThong = (ThanThong)data[2];
            if (thanThong is HoanTran)
            {
                caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Tran, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
            }
            else if (thanThong is HoaCat)
            {
                target.AddBuffAndDebuffCount(EBuffAndDeBuff.Cat, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
            }

        }

    }

    public override string GetDesc(int level)
    {
        return $"Bản thân có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DanLuc)} Khi sử dụng thần thông {GetColoredText($"[Hoàn Trần]", "red")} thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)}. Khi sử dụng thần thông {GetColoredText($"[Hóa Cát]", "red")} thì đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Cat)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams);

    }
}
