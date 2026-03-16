using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(CuuVanLuanHoiQuyet))]

public class CuuVanLuanHoiQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
        {
            10, 15,20,25, 30
        };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // caster gây dame check iscanuse caster

        if(caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoTrao,out SecureInt hoTrao))
        {
            //  giảm dame gây ra 10%
            target.AddDamageData(new List<int> { -(int)(target.allDamages[0]* buffValues[CurrentLevelCongPhapInCombat(caster)-1]/100) });

            
        }
        else
        {
            // tăng dame gây ra 10%
            target.AddDamageData(new List<int> { +(int)(target.allDamages[0] * buffValues[CurrentLevelCongPhapInCombat(caster) - 1] / 100) });

        }



    }
    public virtual void ExecuteParams2(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        // Kiểm tra caster không null và thỏa mãn điều kiện sử dụng
        if (target != null && IsCanUse(target))
        {
            Execute2(caster, target, data);
        }
    }
    public void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // target nhận  dame check iscanuse target

        if (target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoTrao, out SecureInt hoTrao))
        {
            //giảm dame nhận 10%
            target.AddDamageData(new List<int> { -(int)(target.allDamages[0] * buffValues[CurrentLevelCongPhapInCombat(target) - 1] / 100) });


        }
        else
        {
            // tăng dame nhận 10% 
            target.AddDamageData(new List<int> { +(int)(target.allDamages[0] * buffValues[CurrentLevelCongPhapInCombat(target) - 1] / 100) });

        }



    }

    public override string GetDesc(int level)
    {
        return $"Nếu bản thân có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} thì sát thương từ thần thông gây ra và nhận vào giảm xuống {GetColoredText($"{buffValues[level - 1]}%", "red")}. Nếu không, sát thương từ thần thông gây ra và nhận vào tăng lên {GetColoredText($"{buffValues[level - 1]}%", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams2);

    }
}
