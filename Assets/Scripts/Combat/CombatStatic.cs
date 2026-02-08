using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;

public class CombatStatic : MonoBehaviour
{
    public static void TrungDoc(CharacterCombat charSender, CharacterCombat charTrungDoc)
    {
        if (charTrungDoc.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrungDoc, out int trungDoc))
        {
            charTrungDoc.AddDamageData(new List<int> { (int)trungDoc });
            charTrungDoc.TakeDame(charSender);
            Observer.Notify(EventList.GamePlay.TRUNG_DOC, charSender, charTrungDoc);
        }
    }
    public static void SucThe(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        if (charSender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SucThe, out int sucThe))
        {
            charTakeDame.AddDamageData(new List<int> { sucThe });
            charTakeDame.TakeDame(charSender);

        }
       
    }
    public static void KiemKhi(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        if (charSender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out int kiemKhi))
        {
            charTakeDame.AddDamageData(new List<int> { kiemKhi });
            charTakeDame.TakeDame(charSender);

        }
        
    }
}
