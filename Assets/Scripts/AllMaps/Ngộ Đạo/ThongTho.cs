using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThongTho), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(ThongTho))]
public class ThongTho : NgoDaoCombat
{
    [SerializeField] private int buff = 6;
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (BaseSkill.CheckIsEqualETypeSkill(caster, eTypeRequire))
        {
            List<LinhKhiTieuTan> linhKhiTieuTans = caster.thanThongCurrentUse.linhKhiTieuTan;
            int count = 0;
            foreach (var item in linhKhiTieuTans)
            {
                count += item.count;
            }

            target.AddDamageData(new List<int> { target.allDamages[0] * buff * count / 100 });
        }
    }

    public override string GetDes()
    {
        return $"Sử dụng {BaseSkill.GetTextLinhKhi(ELinhKhi.Tho)} hệ thần thông thì tăng lên {BaseSkill.GetColoredText($"({buff} * X)%", "red")} điểm sát thương (X là số linh khí thần thông này tiêu hao)";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
    }
}
