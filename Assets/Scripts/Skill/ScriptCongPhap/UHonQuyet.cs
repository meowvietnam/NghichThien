using DG.Tweening;
using LongDH.LongDHModules;
using LongDHModules;
using Network;
using Network.Extensions;
using Skill;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(UHonQuyet))]

public class UHonQuyet : CPActiveAfterUseSkill, IMemorySerializable<UHonQuyet>
{
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        var congPhapMono = GetCongPhapMono(caster.congPhapEquip);
       
        caster.AddCPDynamic(congPhapMono);
        PlayVisualSkill(caster,target);
    }
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        caster.effectSkillNoDestroy.TryGetValue(this, out var effect);
        if (effect == null)
        {
            base.PlayVisualSkill(caster, target);
            caster.effectSkillNoDestroy.Add(this, ShowSkillEffectParticle(caster, 0, 5,false));

        }

         
      
    }
    
    public void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // target ng nhận dame là ng cần check né tránh
        int index = (int)data[2];
        if (target == null)
            return;
        if (target.allDamages[index] == -1)
            return;
        target.RemoveOneCPDynamic(this);
        target.allDamages[index] = -1;
        if(target.congPhapDynamic.Count(x => x.congPhap == this)<=0)
        {
            target.RemoveEffectSkillNoDestroy(this);

        }    
    }
    public override void InitPosCallExecute(CharacterCombat caster)
    {
        base.InitPosCallExecute(caster);
        Observer.AddObserver(EventList.Combat.CHECK_NE_TRANH, ExecuteParams2, 1);
        PlayVisualSkill(caster,null);
    }
    public virtual void ExecuteParams2 (object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        // Kiểm tra caster không null và thỏa mãn điều kiện sử dụng
        if (target != null && GetCongPhapMono(target.congPhapDynamic) != null)
        {
            Execute2(caster, target, data);
        }
    }

    
    public override string GetDesc(int level)
    {
        return base.GetDesc(level) + $"Tăng lên 1 tầng hồn thể. Mỗi tầng có thể 100% né tránh 1 lần tổn thương, sau đó giải trừ 1 tầng (Bị khắc chế bởi các đòn gây nhiều lần tổn thương)";
    }
    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref UHonQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}
