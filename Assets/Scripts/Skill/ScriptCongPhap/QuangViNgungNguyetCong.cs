using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(QuangViNgungNguyetCong))]

public class QuangViNgungNguyetCong : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
        {
            5,10,20,40,80
        };
    [SerializeField]
    private int[] percents = new int[]
       {
            9,13,17,21,25
       };
    // BEFORE_EXCUTE_SKILL lúc này là caster là ng dùng skill
    // phải check is can use công pháp của target
    // caster than thuc <=0 
    // check caster bị vô hiệu hóa 
    public override void ExecuteParams(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        // Kiểm tra caster không null và thỏa mãn điều kiện sử dụng
        if (target != null && IsCanUse(target))
        {
            Execute(caster, target, data);
        }
    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
       if(caster.GetThanThuc() <=0 )
       {
            int indexLevel = CurrentLevelCongPhapInCombat(target) - 1;
            if (CombatStatic.IsActiveWithPercent(percents[indexLevel]))
            {
                caster.AddSkillVoHieuHoa(this);
                caster.TakeDameByStatic(null, new List<int> { buffValues[indexLevel] });
            }    
       }    


    }

    public override string GetDesc(int level)
    {
        return $"Nếu đối thủ thần thức bé hơn bằng 0, mỗi lần sử dụng thần thông có {GetColoredText($"{percents[level - 1]}%", "red")} tỉ lệ bị vô hiệu hóa. Đồng thời nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.BEFORE_EXCUTE_SKILL, ExecuteParams);

    }
}
