using LongDH.LongDHModules;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(LietBangQuyet))]

public class LietBangQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
     {
        1, 1 , 2 ,2 , 3
     };
    [SerializeField] private int require = 2;
    public override void ExecuteParams(params object[] data)
    {
        CharacterCombat caster = (CharacterCombat)data[0];
        CharacterCombat target = (CharacterCombat)data[1];
        // Tìm trong danh sách trang bị của caster
        if (GetCongPhapMono(caster.congPhapEquip) != null)
        {
            Execute(caster, target, data);

        }
    }
 
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int count = caster.congPhapDynamic.Count(x => x.congPhap is LietBangQuyet);
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SuongGia, out SecureInt  suongGia);
        List<int> dames = (List<int>)data[2];
        int dame = dames[0];
        if(count >= require)
        {
            float percent = (float)buffValues[CurrentLevelCongPhapInCombat(caster)-1] * suongGia / 100f; 
            target.AddDamageData(new List<int> { (int)(dame * percent)});
            RemoveCongPhap(caster);
        }   
        else
        {
            caster.AddCPDynamic(GetCongPhapMono(caster.congPhapEquip));

        }
         


    }
    public override string GetDesc(int level)
    {
        return $"Mỗi khi sử dụng đủ {GetColoredText($"{require} lần", "red")} thần thông gây tổn thương, lần tiếp theo thần thông tạo thành tổn thương tăng lên {GetColoredText($"{buffValues[level - 1]}*X%", "red")} (X là số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SuongGia)} của đối thủ)";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        RemoveCongPhap(caster);
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG, ExecuteParams);

    }
}
