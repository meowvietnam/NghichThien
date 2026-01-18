using LongDH.LongDHModules;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ThanKiemQuyet : CongPhap
{
    [SerializeField] private int[] buff;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {


        if(CheckIsEqualETypeSkill(caster))
        {
            int dame = 0;
            caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out dame);

            dame *= buff[CurrentLevelCongPhapInCombat(caster) - 1];

            target.AddDamageData( new List<int> { dame }, true);
        }


    }
    public void ExecuteParams2(params object[] data)
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
    public void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int count = 0;
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi , out count);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, count);

    }
    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} bây giờ có thể bị gỡ ra. Khi tạo thành {GetTextTypeSkill()} hệ tổn thương thì đối phương nhận thêm sát thương bằng  {GetColoredText($"{buff[level - 1]} lần", "red")} số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} đang có.";
    }


    public override void Init()
    {
        InitPosCallExecute();
    }

    public override void InitPosCallExecute()
    {
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_CACULATE, ExecuteParams);
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ExecuteParams2);

    }
}
