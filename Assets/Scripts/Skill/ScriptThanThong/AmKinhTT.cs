using LongDH.LongDHModules;
using System.Linq;
using Skill;
using UnityEngine;
public abstract class AmKinhTT : ThanThong
{
    [SerializeField] protected int afterHiep = 3;
    public abstract override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data);
   
    public abstract void ExecuteAmKinh(CharacterCombat caster, CharacterCombat target);
        

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.AmKinh)}. Sau {GetColoredText($"{afterHiep} hiệp", "red")}. "+ GetDescAmKinhTT(level);
       
    }
    public abstract string GetDescAmKinhTT(int level);
    

}
