using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(DuongDaoThuat))]
public class DuongDaoThuat : CongPhap
{
    [SerializeField] private int[] dameBuff;

   

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        
            // CHỈ REMOVE KHI ĐÃ THỰC THI BUFF THÀNH CÔNG
            Observer.RemoveObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG, ExecuteParams);

            int dame = dameBuff[CurrentLevelCongPhapInCombat(caster) - 1];
            target.AddDamageData(new List<int> { dame });
      
    }
    public override string GetDesc(int level)
    {
        return $"Mỗi cuộc chiến đấu lần thứ nhất tạo thành tổn thương {GetColoredText($"+{dameBuff[level - 1]}", "red")}";

    }


    public override void Init()
    {
        InitPosCallExecute();
    }

    public override void InitPosCallExecute()
    {
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG, ExecuteParams);
    }
}
