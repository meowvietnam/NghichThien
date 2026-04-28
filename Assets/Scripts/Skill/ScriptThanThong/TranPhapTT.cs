using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;

public abstract class TranPhapTT : ThanThong
{
    [SerializeField] protected List<RequireActive> requireActivesKhuTran;
    [SerializeField]
    protected bool isCanKhuTran => requireActivesKhuTran != null && requireActivesKhuTran.Count>0;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong.GetType() == this.GetType())))
        {
            if (!caster.specialThanThongImpactAfterPlaySkill.Any(x => x.GetType() == this.GetType()))
            {
                caster.AddThanThongImpactAfterPlaySkill(this);


            }
            else
            {
                if (caster.ngoDaoCombats.Any(x => x is KyMon))
                {
                    HauKeVoLuc(caster);
                    ExecuteTranPhap(caster, target);


                }

            }
        }    
        
          

    }
    public override List<RequireActive> GetRequireActive(CharacterCombat caster)
    {
        if(!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong.GetType() == this.GetType())) || !isCanKhuTran)
        {
            return base.GetRequireActive(caster);

        }
        return requireActivesKhuTran;
    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        if ((!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong.GetType() == this.GetType())) || (caster.ngoDaoCombats.Any(x=>x is KyMon) && isCanKhuTran && caster.specialThanThongImpactAfterPlaySkill.Any(x => x.GetType() == this.GetType()))) && base.IsCanUse(caster, target))
        {
            return true;
        }
        return false;
    }

    public abstract void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target);

    public virtual void ObserverEffect(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        if (caster == null) return;

        if (caster.specialThanThongImpactAfterPlaySkill.Any(x => x.GetType() == this.GetType()))
        {
            ExecuteTranPhap(caster,target);

        }


    }
    public override string GetDesc(int level)
    {

        return $"{GetKeyWord(EKeyWord.TranPhap)}. {GetDescTranPhapTT(level)}";

    }
    public abstract string GetDescTranPhapTT(int level);

    public virtual string GetDescTranPhap(CharacterCombat caster , int level)
    {
        if (!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong.GetType() == this.GetType())) || !isCanKhuTran)
        {
            return GetDesc(level);

        }
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Khu động một lần {GetColoredText($"[{this.nameItem}]", "#ffb143")}";
    }    

}
