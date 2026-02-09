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
    public static void NoiThuong(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        if (charTakeDame.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NoiThuong, out int noiThuong))
        {
            charTakeDame.AddDamageData(new List<int> { noiThuong });
            charTakeDame.TakeDame(charSender);

        }

    }
    public static void TrienNhieu(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        if (charTakeDame.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrienNhieu, out int trienNhieu))
        {
            charTakeDame.AddDamageData(new List<int> { trienNhieu });
            charTakeDame.TakeDame(charSender);

        }

    }
    public static bool IsActiveHuyenVu(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        int huyenVu = 0;
        charTakeDame.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HuyenVu, out huyenVu);
        if(huyenVu<=0)
        {
            return false;

        }
        if(IsActiveWithPercent(5))
        {
            return true;
        }    
        return false;

    }
    public static bool IsActiveWithPercent(int percent)
    {
        if (UnityEngine.Random.Range(0, 101) < percent)
        {
            return true;
        }
        return false;
    }
    public static int IndexRandom(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

}
