using LongDH.LongDHModules;
using System.Linq;
using Network;
using Skill;
using UnityEngine;
public class AmKinhTT : ThanThong, IMemorySerializable<AmKinhTT>
{
    [SerializeField] protected int afterHiep = 3;
   
    public virtual void ExecuteAmKinh(CharacterCombat caster, CharacterCombat target) {}
        

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.AmKinh)}. Sau {GetColoredText($"{afterHiep} hiệp", "red")}. "+ GetDescAmKinhTT(level);
       
    }
    public virtual string GetDescAmKinhTT(int level) => string.Empty;

    public void Deserialize(IReader reader, ref AmKinhTT value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }
}
