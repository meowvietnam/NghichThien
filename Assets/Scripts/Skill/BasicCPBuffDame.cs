using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class BasicCPBuffDame : CongPhap
{
    [SerializeField] private int[] dameBuff;



    public override void ExecuteParams(params object[] data)
    {
        // BỎ DÒNG NÀY Ở ĐÂY: Observer.RemoveObserver(..., ExecuteParams);

        // Chỉ gọi base để thực thi logic Execute
        base.ExecuteParams(data);
    }

    public override void Execute(CharacterCombat caster, CharacterCombat target)
    {
        // Kiểm tra hệ của Skill đang dùng có khớp với Công Pháp (SO) này không
        bool isExecute = this.eTypeSkill.Any(skill => caster.thanThongCurrentUse.eTypeSkill.Contains(skill));

        if (isExecute)
        {
            Debug.Log($"<color=cyan>{this.name}</color>: Đúng hệ! Buff dame và tự gỡ bỏ.");

            // CHỈ REMOVE KHI ĐÃ THỰC THI BUFF THÀNH CÔNG
            Observer.RemoveObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_CACULATE, ExecuteParams);

            int dame = dameBuff[CurrentLevelCongPhapInCombat(caster) - 1];
            target.AddDamageData(caster, new List<int> { dame });
        }
        else
        {
            // Không khớp hệ thì im lặng, không Remove để đợi Skill sau trong cùng Turn
            Debug.Log($"<color=orange>{this.name}</color>: Không đúng hệ, tiếp tục chờ...");
        }
    }
    public override string GetDesc(int level)
    {
        return $"Mỗi hiệp lần thứ nhất tạo thành {GetTextTypeSkill()} hệ thần thông sát thương {GetColoredText($"+{dameBuff[level - 1]}", "red")}";

    }
    

    public override void Init()
    {
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, InitPosCallExecute);

    }

    public override void InitPosCallExecute()
    {
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_CACULATE, ExecuteParams);
    }

   
}
