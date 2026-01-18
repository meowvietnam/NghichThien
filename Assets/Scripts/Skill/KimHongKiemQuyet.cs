using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu]
public class KimHongKiemQuyet : CongPhap
{
    [SerializeField] private int[] sucTheBuff;
    [SerializeField] private int[] percentX2Dame;
    [SerializeField] private int requireLinhKhi =4 ;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        if (caster.khiKimUsed >= requireLinhKhi)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.SucThe, sucTheBuff[CurrentLevelCongPhapInCombat(caster) - 1]);
            caster.khiKimUsed -= requireLinhKhi;

        }   
        
    }
    public void Execute2(CharacterCombat caster, CharacterCombat target,List<int> dame)
    {
       
        if (CheckIsEqualETypeSkill(caster) && IsActiveWithPercent(percentX2Dame[CurrentLevelCongPhapInCombat(caster) - 1]))
        {
            
            target.AddDamageData(new List<int> { dame[0] *2 });
        }
    }
    public virtual void ExecuteParams2(params object[] data)
    {
        CharacterCombat caster = (CharacterCombat)data[0];
        CharacterCombat target = (CharacterCombat)data[1];
        List<int> dame = (List<int>)data[2];

        // Tìm trong danh sách trang bị của caster
        if (GetCongPhapMono(caster) != null)
        {
            Execute2(caster, target,dame);

        }
    }
    public override string GetDesc(int level)
    {
        return $"Mỗi tích lũy sử dụng {GetColoredText($"{requireLinhKhi}", "red")} điểm kim hệ linh khí, thu hoạch được {GetColoredText($"+{sucTheBuff[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucThe)}. Tạo thành {GetTextTypeSkill()} hệ thần thông sát thương có tỉ lệ {GetColoredText($"{percentX2Dame[level - 1]}%", "red")} khiến cho sát thương lần này gấp đôi";
    }

    public override void Init()
    {
        InitPosCallExecute();
    }

    public override void InitPosCallExecute()
    {
        Observer.AddObserver(EventList.GamePlay.PLAY_SKILL, ExecuteParams);
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_CACULATE, ExecuteParams2);

    }

}
