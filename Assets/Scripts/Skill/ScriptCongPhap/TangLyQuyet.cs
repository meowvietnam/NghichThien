using System.Collections.Generic;
using UnityEngine;
using LongDH.LongDHModules;
using Skill;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TangLyQuyet))]
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
        SecureInt noiThuong = 0;
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NoiThuong, out noiThuong);

        if(noiThuong >0)
        {
            Debug.Log("tuong dong KimLinhThacThienCong");

            target.AddDamageData(new List<int> { (int)((percentDame / 100f) * dames[0]) });
        }

    }
    public virtual void ExecuteParams2(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        // lúc này là xem bản thân nhận dame là target có skill k nếu có thì sát thương phải nhận tăng lên
        if (target!=null&&IsCanUse(target))
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
        SecureInt noiThuong = 0;
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


  

    public override void InitPosCallExecute(CharacterCombat caster)
    {

        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams2);

    }
}
