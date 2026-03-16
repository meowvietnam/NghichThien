using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(CPBuffDameOrHoTrao))]
public class CPBuffDameOrHoTrao : CongPhap
{
    [SerializeField] private int[] dameBuffPercent;
    [SerializeField] private int[] xHoTraos;


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // Kiểm tra hệ của Skill đang dùng có khớp với Công Pháp (SO) này không
        List<int> dames = (List<int>)data[2];
        int percent = dameBuffPercent[CurrentLevelCongPhapInCombat(caster)-1];
        int xHoTrao = xHoTraos[CurrentLevelCongPhapInCombat(caster) - 1];



        if (CheckIsEqualETypeSkill(caster))
        {
           if(CombatManager.IsTuongDong(caster.thanThongCurrentUse.linhKhiTieuTan))
           {
                Debug.Log("tuong dong KimLinhThacThienCong");

                target.AddDamageData(new List<int> { (int)((percent / 100f) * dames[0]) });
           }
           else if(CombatManager.GetETuongQuan(caster.thanThongCurrentUse.linhKhiTieuTan) == ETuongQuan.TuongSinh)
            {
                Debug.Log("tuong sinh KimLinhThacThienCong");

                int countLinhKhi = 0;
                foreach (var item in caster.thanThongCurrentUse.linhKhiTieuTan)
                {
                    countLinhKhi += item.count;
                }
                caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, countLinhKhi * xHoTrao);
           }
        }
      
    }
    public override string GetDesc(int level)
    {
        return $"Sử dụng {GetTextTypeSkill()} hệ thần thông sát thương. Nếu linh khí được phóng thích là {GetKeyWord(EKeyWord.TuongDong)} thì tạo thành tổn thương tăng thêm {GetColoredText($"{dameBuffPercent[level - 1]}%", "red")}, Còn nếu là {GetKeyWord(EKeyWord.TuongSinh)} thì thu hoạch được {GetColoredText($"+{xHoTraos[level - 1]}*X", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} (X là số linh khí kĩ năng này tiêu hao)";

    }


   

    public override void InitPosCallExecute(CharacterCombat caster)
    {

        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams);
    }

}
