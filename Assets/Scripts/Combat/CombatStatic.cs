using LongDH.LongDHModules;
using Skill;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CombatStatic : MonoBehaviour
{
    public static void ActiveAnimEffect<T>(CharacterCombat charActive, EBuffAndDeBuff buffFind, Action<T> action)
        where T : BuffEffect
    {
        // Search specifically for components of type T to ensure the Action matches
        var listBuff = charActive.GetComponentsInChildren<T>();

        foreach (var item in listBuff)
        {
            // Check if the buff type matches the enum
            if (item.eBuff == buffFind)
            {
                item.ShowEffect();

                // Invoke the callback action passed into the method
                action?.Invoke(item);

                break;
            }
        }
    }

    public static void TrungDoc(CharacterCombat charSender, CharacterCombat charTrungDoc)
    {
        if (charTrungDoc.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrungDoc, out SecureInt trungDoc))
        {
            charTrungDoc.TakeDameByStatic(charSender, new List<int> { (int)trungDoc }, false,
                new[] { BaseSkill.ETypeSkill.Moc });

            Observer.Notify(EventList.Combat.TRUNG_DOC, charSender, charTrungDoc);
            ActiveAnimEffect<BuffEffect>(charTrungDoc, EBuffAndDeBuff.TrungDoc, null);
        }
    }

    public static void ThieuDot(CharacterCombat charSender, CharacterCombat charThieuDot)
    {
        if (charThieuDot.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThieuDot, out SecureInt thieuDot))
        {
            ActiveAnimEffect<BuffEffect>(charThieuDot, EBuffAndDeBuff.ThieuDot, null);

            if (charThieuDot.ngoDaoCombats.FirstOrDefault(x => x is DucHoa) is DucHoa ducHoa
                && ducHoa.IsCanUse(charThieuDot))
            {
                // If DucHoa exists and can be used, heal the character
                charThieuDot.Healing(thieuDot);
            }
            else
            {
                // Otherwise, apply damage
                charThieuDot.TakeDameByStatic(
                    charSender,
                    new List<int> { (int)thieuDot },
                    false,
                    new[] { BaseSkill.ETypeSkill.Hoa }
                );
            }


            // Observer.Notify(EventList.GamePlay.TRUNG_DOC, charSender, charThieuDot);
        }
    }

    public static void SucThe(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        if (charSender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SucThe, out SecureInt sucThe))
        {
            charTakeDame.TakeDameByStatic(charSender, new List<int> { sucThe }, false,
                new[] { BaseSkill.ETypeSkill.Kiem });

            ActiveAnimEffect<SucTheEffect>(charSender, EBuffAndDeBuff.SucThe,
                (SucTheEffect) => { SucTheEffect.ShowSucTheAtk(); });
        }
    }

    public static void KiemKhi(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        if (charSender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out SecureInt kiemKhi))
        {
            charTakeDame.TakeDameByStatic(charSender, new List<int> { kiemKhi }, false,
                new[] { BaseSkill.ETypeSkill.Kiem });
            ActiveAnimEffect<BuffEffectTriggerAttack>(charSender, EBuffAndDeBuff.KiemKhi,
                (buff) => { buff.ActiveAnim(); });

            Observer.Notify(EventList.Combat.KIEM_KHI, charSender, charTakeDame);
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
            charTakeDame.TakeDameByStatic(charSender, new List<int> { (int)anhDich }, false,
                new[] { BaseSkill.ETypeSkill.Khi });

            charSender.Healing(anhDich);
            ActiveAnimEffect<BuffEffectTriggerAttack>(charSender, EBuffAndDeBuff.AnhDich,
                (buff) => { buff.ActiveAnim(); });
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
            charTakeDame.TakeDameByStatic(charSender, new List<int> { (int)trienNhieu }, false,
                new[] { BaseSkill.ETypeSkill.Moc });
            Debug.Log("Static TrienNhieu");
        }
    }

    public static void ChayMau(CharacterCombat charSender, CharacterCombat charTakeDame)
    {
        if (charTakeDame.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChayMau, out SecureInt chayMau))
        {
            charTakeDame.TakeDameByStatic(charSender,
                new List<int>
                {
                    (int)(chayMau * 0.01f *
                          (charTakeDame.characterStatsTemp.hpMax - charTakeDame.characterStatsTemp.hp))
                });
            Debug.Log("Static ChayMau");
        }
    }

    public static void QuyTon(CharacterCombat charActive, CharacterCombat charTakeDame)
    {
        void ActiveQuyTon(int countDebuff, int resetHonLuc, EBuffAndDeBuff eBuff)
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
                charActive.RemoveBuffAndDebuffCount(EBuffAndDeBuff.HonLuc, honLuc);
            }

            charActive.AddBuffAndDebuffCount(EBuffAndDeBuff.HonLuc, resetHonLuc);
            ActiveAnimEffect<BuffEffectTriggerAttack>(charActive, eBuff, (buff) => { buff.ActiveAnim(); });
        }

        int valueBuffHpQuy = (int)((ThanThong.GetDaiCanhGioi(charActive.characterStatsBase.level.Value) * 0.1f) *
                                   charActive.characterStatsBase.hpMax.Value);
        if (charActive.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.QuyLinh, out SecureInt QuyLinh))
        {
            ActiveQuyTon(1, 150 + valueBuffHpQuy, EBuffAndDeBuff.QuyLinh);
        }

        if (charActive.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.QuyBinh, out SecureInt QuyBinh))
        {
            ActiveQuyTon(2, 300 + valueBuffHpQuy, EBuffAndDeBuff.QuyBinh);
        }

        if (charActive.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.QuyTuong, out SecureInt QuyTuong))
        {
            ActiveQuyTon(3, 500 + valueBuffHpQuy, EBuffAndDeBuff.QuyTuong);
        }

        if (charActive.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.QuyTon, out SecureInt quyTon))
        {
            ActiveQuyTon(4, 1500 + valueBuffHpQuy, EBuffAndDeBuff.QuyTon);
        }

        if (charActive.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.QuyDe, out SecureInt QuyDe))
        {
            ActiveQuyTon(5, 5000 + valueBuffHpQuy, EBuffAndDeBuff.QuyDe);
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
                Observer.Notify(EventList.Combat.UPDATE_AMKINH_THANTHONG, currentTurn, entry);
            }
        }

        foreach (var entry in entryRemoves)
        {
            currentTurn.listAmKinhTT.Remove(entry);
            Observer.Notify(EventList.Combat.UPDATE_AMKINH_THANTHONG, currentTurn, entry, -1);
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
        if (huyenVu <= 0)
        {
            return false;
        }

        if (IsActiveWithPercent(5))
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