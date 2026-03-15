using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(NhienHonQuyet))]

public class NhienHonQuyet : CongPhap
{

    [SerializeField]
    private int[] buffValues = new int[]
         {
            1, 2 , 3,4, 5
         };

    [SerializeField] private int debuff = 1;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (caster.thanThongCurrentUse.thanThong is KinhHonChu)
        {
            target.AddDamageData(new List<int> { buffValues[CurrentLevelCongPhapInCombat(caster)-1] });

        }

    }
    public virtual void ExecuteParams2(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        // Kiểm tra caster không null và thỏa mãn điều kiện sử dụng
        if (caster != null && IsCanUse(caster))
        {
            Execute2(caster, target, data);
        }
    }
    public void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.characterStatsBase.thanThuc -= debuff;

    }
    public override string GetDesc(int level)
    {
        return $"Thần thông [Kinh Hồn Chú] tạo thành tổn thương {GetColoredText($"+{buffValues[level - 1]}", "red")}. Sử dụng công pháp này khi kết thúc chiến đấu thì thần thức {GetColoredText($"-{debuff}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.CACULATE_DAME_BY_THANTHONG, ExecuteParams);
        Observer.AddObserver(EventList.GamePlay.END_COMBAT, ExecuteParams2);

    }
}
