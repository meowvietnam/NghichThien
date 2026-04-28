using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TamTrai), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(TamTrai))]
public class TamTrai : NgoDaoCombat
{
    [SerializeField] private int buff = 2;
    [SerializeField] private int requireSave = 3;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(BaseSkill.CheckIsEqualETypeSkill(caster,eTypeRequire))
        {
            if(caster.ngoDaoCombatsDynamic.Count(x => x is TamTrai) >= requireSave)
            {
                ThanThong thanThong = (ThanThong)data[2];
                if(thanThong is BaseSkill.ThanThuc thanThucSkill)
                {
                    for(int i = 0; i < buff; i++)
                    {
                        thanThucSkill.ExecuteThanThuc(caster, target);

                    }
                    caster.RemoveNgoDaoCombatDynamic(this);
                }    
            }
            else
            {
                caster.AddNgoDaoCombatDynamic(this);
            }    
        }    
    }

    public override string GetDes()
    {
        return $"Mỗi tích lũy sử dụng {BaseSkill.GetColoredText($"{requireSave}", "red")} lần thần thức hệ thần thông, thì lần tiếp theo [Thần Thức] đặc hiệu của thần thông sẽ phát động {BaseSkill.GetColoredText($"{buff}", "red")} lần";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ObserverEffect);
    }
}
