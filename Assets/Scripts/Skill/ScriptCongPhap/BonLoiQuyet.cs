using LongDH.LongDHModules;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(BonLoiQuyet))]
public class BonLoiQuyet : CongPhap
{
    [SerializeField] private int[] sucTheBuff;
    [SerializeField] private int require = 5;
    public override void ExecuteParams(params object[] data)
    {
        Debug.Log("Vao day0");
        List<LinhKhiTieuTan> linhKhiTieuTan = data[3] as List<LinhKhiTieuTan>;
        if (IsCanActive(linhKhiTieuTan))
        {
            Debug.Log("Vao day1");

            base.ExecuteParams(data);

        }
    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        Debug.Log("Vao day2");

        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.SucThe, sucTheBuff[CurrentLevelCongPhapInCombat(caster)-1]);

    }

    public override string GetDesc(int level)
    {
        return $"Sử dụng thần thông có linh khí tiêu hao lớn hơn hoặc bằng {GetColoredText($"{require}", "red")} điểm thì thu hoạch được {GetColoredText($"+{sucTheBuff[level - 1]}", "red")}";

    }
    private bool IsCanActive(List<LinhKhiTieuTan> linhKhiTieuTan)
    {
        int count = 0;
        foreach (var item in linhKhiTieuTan)
        {
            count += item.count;
        }
        if(count>= require)
        {
            return true;
        }
        return false;
    }    


   

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.PLAY_SKILL, ExecuteParams);
    }


}
