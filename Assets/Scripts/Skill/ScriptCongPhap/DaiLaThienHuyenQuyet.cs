using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(DaiLaThienHuyenQuyet))]
public class DaiLaThienHuyenQuyet : CongPhap
{

   
    [SerializeField]
    private int[] requires = new int[]
        {
            10, 9 , 8,7, 5
        };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        if(caster.congPhapDynamic.Count(x=>x.congPhap is DaiLaThienHuyenQuyet) >= requires[CurrentLevelCongPhapInCombat(caster)-1])
        {
            RemoveCongPhap(caster);
            target.AddDamageData(new List<int> { target.allDamages[0] });
        }
        else
        {
            caster.AddCPDynamic(GetCongPhapMono(caster.congPhapEquip));

        }
    }
    public override void ExecuteParams(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        // Kiểm tra caster không null và thỏa mãn điều kiện sử dụng
        if (caster != null && GetCongPhapMono(caster.congPhapEquip) != null)
        {
            Execute(caster, target, data);
        }
    }
   
    public override string GetDesc(int level)
    {
        return $"Mỗi sử dụng {GetColoredText($"{requires[level - 1]}", "red")} lần thần thông gây tổn thương thì lần tiếp theo tổn thương gấp đôi";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams);
        RemoveCongPhap(caster);

    }
}
