using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TatPhongKinh))]

public class TatPhongKinh : PhiVanKinh
{
    [SerializeField]
    private int[] buffValues2 = new int[]
       {
            16, 32 , 66,136, 278
       };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(caster.characterStatsCurrent.tocDoBay > target.characterStatsCurrent.tocDoBay)
        {
            target.AddDamageData(new System.Collections.Generic.List<int> { buffValues2[CurrentLevelCongPhapInCombat(caster)-1] });
            RemoveCongPhap(caster);
        }    
    }

    public override string GetDesc(int level)
    {
        return $"{base.GetDesc(level)}. Nếu độn tốc lớn hơn đối thủ, thì lần đầu sử dụng thần thông tổn thương tăng lên {GetColoredText($"+{buffValues2[level - 1]}", "red")} (Mỗi cuộc chiến chỉ có tác dụng một lần)";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.CACULATE_DAME_BY_THANTHONG, ExecuteParams);

    }
}
