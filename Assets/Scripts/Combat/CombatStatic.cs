using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CombatStatic : MonoBehaviour
{
    public static void TrungDoc(CharacterCombat charSender, CharacterCombat charTrungDoc)
    {
        if (charTrungDoc.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrungDoc, out SecureInt trungDoc))
        {
            charTrungDoc.TakeDameByStatic(charSender, new List<int> { (int)trungDoc }, false, new[] {BaseSkill.ETypeSkill.Moc});
           
            Observer.Notify(EventList.GamePlay.TRUNG_DOC, charSender, charTrungDoc);
            var listBuff = charTrungDoc.GetComponentsInChildren<BuffEffect>();
            foreach (var item in listBuff)
            {
                if (item.eBuff == EBuffAndDeBuff.TrungDoc)
                {
                    item.ShowEffect();
                    break;
                }
            }
        }
    }
    public static void ThieuDot(CharacterCombat charSender, CharacterCombat charThieuDot)
    {
        if (charThieuDot.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThieuDot, out SecureInt thieuDot))
        {
            charThieuDot.TakeDameByStatic(charSender, new List<int> { (int)thieuDot }, false, new[] { BaseSkill.ETypeSkill.Hoa });
            var listBuff = charThieuDot.GetComponentsInChildren<BuffEffect>();
            foreach (var item in listBuff)
            {
                if (item.eBuff == EBuffAndDeBuff.ThieuDot)
                {
                    item.ShowEffect();
                    break;
                }
            }
            // Observer.Notify(EventList.GamePlay.TRUNG_DOC, charSender, charThieuDot);
        }
    }
    public static void SucThe(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        if (charSender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SucThe, out SecureInt sucThe))
        {
            charTakeDame.TakeDameByStatic(charSender, new List<int> { sucThe }, false, new[] { BaseSkill.ETypeSkill.Kiem });

            var listBuff = charSender.GetComponentsInChildren<SucTheEffect>();

            foreach (var item in listBuff)
            {
                if (item.eBuff == EBuffAndDeBuff.SucThe)
                {
                    item.ShowSucTheAtk();
                    break;
                }
            }
        }

    }
    public static void KiemKhi(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        if (charSender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out SecureInt kiemKhi))
        {
            charTakeDame.TakeDameByStatic(charSender, new List<int> { kiemKhi }, false, new[] { BaseSkill.ETypeSkill.Kiem });
            var listBuff = charSender.GetComponentsInChildren<BuffEffect>();
            foreach (var item in listBuff)
            {
                if(item.eBuff == EBuffAndDeBuff.KiemKhi)
                {
                    item.anim.SetTrigger("Active");
                    break;
                }    
            }
            Observer.Notify(EventList.GamePlay.KIEM_KHI, charSender, charTakeDame);

        }

    }
    public static void ThienKiem(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        if (charSender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThienKiem, out SecureInt thienKiem))
        {
            for (int i = 0; i < thienKiem; i++)
            {
                KiemKhi(charSender, charTakeDame);
            }

        }

    }
    public static void AnhDich(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        if (charSender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.AnhDich, out SecureInt anhDich))
        {
            charTakeDame.TakeDameByStatic(charSender, new List<int> { (int)anhDich }, false, new[] { BaseSkill.ETypeSkill.Khi });

            charSender.Healing(anhDich);

        }

    }
    public static void NoiThuong(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        if (charTakeDame.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NoiThuong, out SecureInt noiThuong))
        {
            charTakeDame.TakeDameByStatic(charSender, new List<int> { (int)noiThuong }, false);
            Debug.Log("Static NoiThuong");


        }

    }
    public static void TrienNhieu(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        if (charTakeDame.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrienNhieu, out SecureInt trienNhieu))
        {
            charTakeDame.TakeDameByStatic(charSender, new List<int> { (int)trienNhieu }, false, new[] { BaseSkill.ETypeSkill.Moc });
            Debug.Log("Static TrienNhieu");

        }

    }
    public static void QuyTon(CharacterCombat charActive, CharacterCombat charTakeDame)
    {
        void ActiveQuyTon(int countDebuff , int resetHonLuc)
        {
            for (int i = 0; i < countDebuff; i++)
            {
                var debuff = BaseSkill.eDebuffs[CombatStatic.IndexRandom(0, BaseSkill.eDebuffs.Count)];
                charTakeDame.AddBuffAndDebuffCount(debuff, 1);
            }
            charActive.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Hon, out SecureInt hon);
            charActive.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hon);

            if (charActive.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HonLuc, out SecureInt honLuc))
            {
                charActive.RemoveBuffAndDebuffCount(EBuffAndDeBuff.HonLuc,honLuc);

            }
            charActive.AddBuffAndDebuffCount(EBuffAndDeBuff.HonLuc,resetHonLuc);
        }
        if (charActive.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.QuyLinh, out SecureInt QuyLinh))
        {
            ActiveQuyTon(1,150);
        }
        if (charActive.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.QuyBinh, out SecureInt QuyBinh))
        {
            ActiveQuyTon(2,300);

        }
        if (charActive.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.QuyTuong, out SecureInt QuyTuong))
        {
            ActiveQuyTon(3,500);

        }
        if (charActive.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.QuyTon, out SecureInt quyTon))
        {
            ActiveQuyTon(4,1500);

        }
        if (charActive.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.QuyDe, out SecureInt QuyDe))
        {
            ActiveQuyTon(5,5000);

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
