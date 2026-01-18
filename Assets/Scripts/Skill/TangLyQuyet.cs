using System.Collections.Generic;
using UnityEngine;
using LongDH.LongDHModules;
[CreateAssetMenu]
public class TangLyQuyet : CongPhap
{
    [SerializeField] private int[] dameBuffPercent;
    [SerializeField] private int[] dameDebuffPercent;


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int level = CurrentLevelCongPhapInCombat(caster);
        if (level == 0) return;
        List<int> dames = (List<int>)data[2];
        Debug.Log($"dcmmm + lv "+ CurrentLevelCongPhapInCombat(caster) + " length là " + dameBuffPercent.Length);
        
        int percentDame = dameBuffPercent[level - 1];
        int noiThuong = 0;
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NoiThuong, out noiThuong);

        if(noiThuong >0)
        {
            Debug.Log("tuong dong KimLinhThacThienCong");

            target.AddDamageData(new List<int> { (int)((percentDame / 100f) * dames[0]) });
        }

    }
    public virtual void ExecuteParams2(params object[] data)
    {
        CharacterCombat caster = (CharacterCombat)data[0];
        CharacterCombat target = (CharacterCombat)data[1];
        // Tìm trong danh sách trang bị của caster
        if (GetCongPhapMono(target) != null)
        {
            Execute2(caster, target, data);

        }
    }
    public void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int level = CurrentLevelCongPhapInCombat(target);

        if (level == 0)
        {
            Debug.Log("dung excute 2 tanglyQuyet" + target.gameObject.name);

            return;
        }
        
        List<int> dames = (List<int>)data[2];
        int percentDame = dameDebuffPercent[level - 1];
        int noiThuong = 0;
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NoiThuong, out noiThuong);

        if (noiThuong > 0)
        {
            Debug.Log("excute 2 tanglyQuyet");

            target.AddDamageData(new List<int> { (int)((percentDame / 100f) * dames[0]) });
        }

    }
    public override string GetDesc(int level)
    {
        return $"Bản thân có trạng thái nội thương, bản thân nhận sát thương tăng lên {GetColoredText($"{dameDebuffPercent[level - 1]}%", "red")},nhưng có thể tạo thành sát thương thần thông tăng lên là {GetColoredText($"{dameBuffPercent[level - 1]}%", "red")}";

    }


    public override void Init()
    {
        InitPosCallExecute();

    }

    public override void InitPosCallExecute()
    {

        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_CACULATE, ExecuteParams);
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_CACULATE, ExecuteParams2);

    }
}
