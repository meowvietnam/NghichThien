using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatStatic : MonoBehaviour
{
    public static void TrungDoc(CharacterCombat charSender, CharacterCombat charTrungDoc)
    {
        if (charTrungDoc.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrungDoc, out SecureInt trungDoc))
        {
            charTrungDoc.AddDamageData(new List<int> { (int)trungDoc });
            charTrungDoc.TakeDame(charSender);
            Observer.Notify(EventList.GamePlay.TRUNG_DOC, charSender, charTrungDoc);
        }
    }
    public static void ThieuDot(CharacterCombat charSender, CharacterCombat charThieuDot)
    {
        if (charThieuDot.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThieuDot, out SecureInt thieuDot))
        {
            charThieuDot.AddDamageData(new List<int> { (int)thieuDot });
            charThieuDot.TakeDame(charSender);
           // Observer.Notify(EventList.GamePlay.TRUNG_DOC, charSender, charThieuDot);
        }
    }
    public static void SucThe(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        if (charSender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SucThe, out SecureInt sucThe))
        {
            charTakeDame.AddDamageData(new List<int> { sucThe });
            charTakeDame.TakeDame(charSender);

        }
       
    }
    public static void KiemKhi(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        if (charSender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out SecureInt kiemKhi))
        {
            charTakeDame.AddDamageData(new List<int> { kiemKhi });
            charTakeDame.TakeDame(charSender);

        }
        
    }
    public static void AnhDich(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        if (charSender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.AnhDich, out SecureInt anhDich))
        {
            charTakeDame.AddDamageData(new List<int> { anhDich });
            charTakeDame.TakeDame(charSender);
            charSender.Healing(anhDich);

        }

    }
    public static void NoiThuong(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        if (charTakeDame.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NoiThuong, out SecureInt noiThuong))
        {
            charTakeDame.AddDamageData(new List<int> { noiThuong });
            charTakeDame.TakeDame(charSender);

        }

    }
    public static void TrienNhieu(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        if (charTakeDame.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrienNhieu, out SecureInt trienNhieu))
        {
            charTakeDame.AddDamageData(new List<int> { trienNhieu });
            charTakeDame.TakeDame(charSender);

        }

    }
    public static void AmKinhTTNewTurn(CharacterCombat currentTurn, CharacterCombat targetTurn)
    {
        List<CharacterCombat.AmKinhEntry> entryRemoves = new List<CharacterCombat.AmKinhEntry>();
        // Duyệt ngược từ cuối danh sách lên đầu để xóa an toàn khi đang lặp
        for (int i = currentTurn.listAmKinhTT.Count - 1; i >= 0; i--)
        {
            var entry = currentTurn.listAmKinhTT[i];

            if (entry.RemainingTurns <= 1)
            {
                // Kích hoạt action
                entry.ThanThong.ExecuteAmKinh(currentTurn, targetTurn);
                entryRemoves.Add(entry);
            }
            else
            {
                // Giảm số hiệp chờ
                entry.RemainingTurns -= 1;
            }
        }
        foreach(var entry in entryRemoves)
        {
            currentTurn.listAmKinhTT.Remove(entry);
        }    

    }
    public static bool IsActiveAnNac(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        charTakeDame.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.AnNac, out SecureInt anNac);
        if (anNac <= 0)
        {
            return false;

        }
        if (IsActiveWithPercent(50))
        {
            return true;
        }
        return false;

    }
    public static bool IsActiveAnhNac(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        charTakeDame.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.AnhNac, out SecureInt anhNac);
        if (anhNac <= 0)
        {
            return false;

        }
        if (IsActiveWithPercent(9))
        {
            return true;
        }
        return false;

    }
    public static bool IsActiveHuyenVu(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        SecureInt huyenVu = 0;
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
