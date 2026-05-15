using System.Collections.Generic;
using System.Linq;
using Network;
using Skill;
using UnityEngine;

public class TranPhapTT : ThanThong, IMemorySerializable<TranPhapTT>
{
    [SerializeField] protected List<RequireActive> requireActivesKhuTran;
    [SerializeField]
    protected bool isCanKhuTran => requireActivesKhuTran != null && requireActivesKhuTran.Count>0;
    [SerializeField] private float delayTimeEf = 2f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong.Id == this.Id)))
        {
            if (prefabEffectSkill != null)
            {
                ShowSkillEffectWorld(caster, delayTimeEf);
            }
            if (!caster.specialThanThongImpactAfterPlaySkill.Any(x => x.Id == this.Id))
            {
                caster.AddThanThongImpactAfterPlaySkill(this);


            }
            else
            {
                if (caster.ngoDaoCombats.Any(x => x is KyMon))
                {
                    HauKeVoLuc(caster);
                    OnExecuteTranPhap(caster, target);


                }

            }
        }    
        
          

    }
    public override List<RequireActive> GetRequireActive(CharacterCombat caster)
    {
        if(!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong.Id == this.Id)) || !isCanKhuTran)
        {
            return base.GetRequireActive(caster);

        }
        return requireActivesKhuTran;
    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        if ((!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong.Id == this.Id)) || (caster.ngoDaoCombats.Any(x=>x is KyMon) && isCanKhuTran && caster.specialThanThongImpactAfterPlaySkill.Any(x => x.Id == this.Id))) && base.IsCanUse(caster, target))
        {
            return true;
        }
        return false;
    }
    public void OnExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        if (prefabEffectSkill != null)
        {
            ShowSkillEffectWorld(caster, delayTimeEf);
        }
        ExecuteTranPhap(caster,target);
    }    

    public virtual void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target) {}

    public virtual void ObserverEffect(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        if (caster == null) return;

        if (caster.specialThanThongImpactAfterPlaySkill.Any(x => x.Id == this.Id))
        {
            OnExecuteTranPhap(caster,target);

        }
         

    }
    public override string GetDesc(int level)
    {

        return $"{GetKeyWord(EKeyWord.TranPhap)}. {GetDescTranPhapTT(level)}";

    }
    public virtual string GetDescTranPhapTT(int level) => string.Empty;

    public virtual string GetDescTranPhap(CharacterCombat caster , int level)
    {
        if (!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong.Id == this.Id)) || !isCanKhuTran)
        {
            return GetDesc(level);

        }
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Khu động một lần {GetColoredText($"[{this.nameItem}]", "#ffb143")}";
    }

    public void Deserialize(IReader reader, ref TranPhapTT value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }
}
