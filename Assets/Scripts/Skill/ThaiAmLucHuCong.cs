using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class ThaiAmLucHuCong : CongPhap
{
    [SerializeField] private int[] buff;
    [SerializeField] private int buff2;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.DeTonThuong, buff[CurrentLevelCongPhapInCombat(caster) - 1]);


    }
    public  void ExecuteParams2(params object[] data)
    {
        // target là ng nh?n dame 
        // ktra xem ng nh?n dame có công pháp này k

        CharacterCombat caster = (CharacterCombat)data[0];
        CharacterCombat target = (CharacterCombat)data[1];
        // Tìm trong danh sách trang b? c?a caster
        if (GetCongPhapMono(caster) != null)
        {
            Execute2(caster, target, data);

        }
    }
    public  void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        target.AddBuffAndDebuffCount(EBuffAndDeBuff.DeTonThuong, buff2);

    }
    public override string GetDesc(int level)
    {
        return $"Bắt đầu hiệp, địch nhân nhận phải {GetColoredText($"+{buff[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DeTonThuong)}. Mỗi lần sử dụng thần thông gây tổn thương, khiến đối phương nhận phải {GetColoredText($"+{buff2}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DeTonThuong)}";
    }


    public override void Init()
    {
        InitPosCallExecute();
    }

    public override void InitPosCallExecute()
    {
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ExecuteParams);
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_CACULATE, ExecuteParams2);

    }
}
