using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(PhuTranLuyenTamQuyet))]

public class PhuTranLuyenTamQuyet : BangTamQuyet
{
    [SerializeField]
    private int[] buffValues2 = new int[]
       {
            9, 13 , 17,21, 25
       };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NhieuHon,out SecureInt nhieuHon)&& CheckIsEqualETypeSkill(caster))
        {
            target.AddDamageData(new List<int> { (int)(target.allDamages[0] * buffValues2[CurrentLevelCongPhapInCombat(caster) - 1] / 100) });
        }    
    }

    public override string GetDesc(int level)
    {
        return $"{base.GetDesc(level)}. Nếu đối thủ ở trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NhieuHon)} thì sử dụng {GetTextTypeSkill(ETypeSkill.Than)} hệ thần thông tổn thương tăng lên {GetColoredText($"{buffValues2[level - 1]}%", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.CACULATE_DAME_BY_THANTHONG, ExecuteParams);

    }
}
