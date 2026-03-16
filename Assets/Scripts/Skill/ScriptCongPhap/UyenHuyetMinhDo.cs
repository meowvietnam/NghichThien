using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(UyenHuyetMinhDo))]

public class UyenHuyetMinhDo : CPTieuHaoThoNguyen
{
   
    [SerializeField]
    private int[] debuffValues = new int[]
       {
            4, 6 , 8,10, 12
       };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int indexLevel = CurrentLevelCongPhapInCombat(caster) - 1;
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.ChayMau, debuffValues[indexLevel]);
        RemoveCongPhap(caster);
         

    }
    public virtual void ExecuteParams2(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        // Kiểm tra caster không null và thỏa mãn điều kiện sử dụng
        if (caster != null && GetCongPhapMono(caster.congPhapEquip) != null)
        {
            Execute2(caster, target, data);
        }
    }
    public  void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // target là người nhận dame

        // caster người gây dame

        // iscanuse là caster 

        // nếu target có chảy máu
        int dame = (int)data[2];
        if (target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChayMau , out SecureInt chayMau))
        {
            caster.Healing(dame);
        }
        


    }
    public override string GetDesc(int level)
    {
        return $"Bắt đầu chiến đấu, đối thủ nhận phải {GetColoredText($"+{debuffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChayMau)}. Nếu đối thủ có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChayMau)} thì khôi phục HP tương đương lượng tổn thương gây ra. {base.GetDesc(level)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ExecuteParams);

        Observer.AddObserver(EventList.Combat.CHARACTER_TAKE_DAME, ExecuteParams2);

    }
}
