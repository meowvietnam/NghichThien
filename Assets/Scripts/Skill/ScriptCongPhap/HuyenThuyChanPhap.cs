using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HuyenThuyChanPhap))]

public class HuyenThuyChanPhap : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
    {
        1 , 1 , 2 ,3 , 3
    };
    public override void ExecuteParams(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;        // Tìm trong danh sách trang bị của caster
        if (target != null && IsCanUse(target))
        {
            Execute(null, target, data);

        }
    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int level = CurrentLevelCongPhapInCombat(target);
        if(level == 1)
        {
            if(CombatStatic.IsActiveWithPercent(50))
            {
                target.Healing(buffValues[level - 1]);

            }    
        }    
        else if(level == buffValues.Length)
        {
            float remainHp = target.characterStatsMax.hp - target.characterStatsCurrent.hp;
            int heal = buffValues[level - 1];
            float healRemain = remainHp - heal;
            if(healRemain < 0) // tức là hồi full máu còn dư ra
            {

                if (CombatStatic.IsActiveWithPercent(75))
                {
                    target.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao,Mathf.Abs((int)healRemain));
                }

            }
            target.Healing(heal);

          
        }
        else
        {
            target.Healing(buffValues[level - 1]);

        }

    }

    public override string GetDesc(int level)
    {
        if(level == 1)
        {
            return $"Bị tổn thương, có 50% khôi phục {GetColoredText($"{buffValues[level - 1]}", "red")} điểm Hp";

        }
        else if(level == buffValues.Length)
        {
            return $"Bị tổn thương, Khôi phục {GetColoredText($"{buffValues[level - 1]}", "red")} điểm Hp. Nếu đã đầy Hp thì có 75% chuyển hóa lượng hồi phục còn thừa thành hộ tráo";

        }
        return $"Bị tổn thương, Khôi phục {GetColoredText($"{buffValues[level - 1]}", "red")} điểm Hp";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME, ExecuteParams);
    }
}
