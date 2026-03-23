using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(DanLuc), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(DanLuc))]
public class DanLuc : NgoDaoCombat
{


    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(BaseSkill.CheckIsEqualETypeSkill(caster,eTypeRequire)&&target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DanLuc,out SecureInt danLuc))
        {
            List<LinhKhiTieuTan> linhKhiTieuTans = caster.thanThongCurrentUse.linhKhiTieuTan;
            int count = 0;
            foreach (var item in linhKhiTieuTans)
            {
                count += item.count; 
            }

            target.AddDamageData(new List<int> { danLuc * count });
        }    
    }

    public override string GetDes()
    {
        return $"Đối thủ mỗi có một tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DanLuc)} thì sử dụng {BaseSkill.GetTextLinhKhi(ELinhKhi.Tho)} hệ thần thông thì tăng lên {BaseSkill.GetColoredText($"X", "red")} điểm sát thương (X là số linh khí thần thông này tiêu hao)";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
    }

}
