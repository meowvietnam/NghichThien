using AllMaps;
using LongDH.LongDHModules;
using NUnit.Framework;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Networking.UnityWebRequest;


public class CharacterStats
{    // chỉ số
    public float percentKim;
    public float percentMoc;
    public float percentThuy;
    public float percentHoa;
    public float percentTho;
    public float percentMa;

    public int level;
    public float hp;
    public float nameUser;
    public int tuoiTac;
    public int thoNguyen;
    public int tuChat;
    public int thanThuc;
    public int ngoTinh;
    public int tocDoBay;
    public int tamCanh;
    public int danDoc;
    public int money;
    public int idLinhCan;
    // tông môn
    public int chucVi;
    public int monPhai;
    // skill
    public int[] thanThongOwner;
    public int[] thanThongEquip;
    public class CongPhapSave
    {
        public int id;
        public int level;
    }
    public CongPhapSave congPhapOwner;
    // công pháp used
    public int donThuatUsed;
    public int phuTu1Used;
    public int phuTu2Used;
    public int phuTu3Used;
    public int phuTu4Used;
    public int chuTu1Used;
    public int chuTu2Used;
   
    public CharacterStats Clone()
    {
        return (CharacterStats)this.MemberwiseClone();
    }



}
public class CharacterCombat : MonoBehaviour
{
    public int khiKim;
    public int khiMoc;
    public int khiThuy;
    public int khiHoa;
    public int khiTho;
    public int khiMa;

    public CharacterStats characterStats;
    public CharacterStats characterStatsCurrent;
    public ThanThong thanThongOldUse;

    public class CountStack<T>
    {
        public T type;
        public int count;
    }
    public Dictionary<ThanThong , int> thanThongCoolDown = new Dictionary<ThanThong,int>();
    public Dictionary<EBuffAndDeBuff, int> buffAndDebuffCount = new  Dictionary <EBuffAndDeBuff, int>();

    public void AddThanThongCoolDown(ThanThong thanThong , int count)
    {
        thanThongCoolDown[thanThong] = thanThongCoolDown.GetValueOrDefault(thanThong, 0) + count;

    }
    public void AddBuffAndDebuffCount(EBuffAndDeBuff type, int count)
    {

        buffAndDebuffCount[type] =
        buffAndDebuffCount.TryGetValue(type, out var current)
        ? current + count
        : count;

        Observer.Notify(EventList.GamePlay.ADD_BUFF, this, type, buffAndDebuffCount[type]);


    }
    public void RemoveBuffAndDebuffCount(EBuffAndDeBuff type, int count)
    {
        if (!buffAndDebuffCount.TryGetValue(type, out int current))
            return; // không có buff thì khỏi remove

        int newValue = current - count;

        if (newValue <= 0)
        {
            buffAndDebuffCount.Remove(type);
            newValue = 0;
        }
        else
        {
            buffAndDebuffCount[type] = newValue;
        }

        Observer.Notify(EventList.GamePlay.ADD_BUFF, this, type, newValue);
    }
    public void Init(CharacterStats characterStat)
    {
        characterStats = characterStat.Clone();
        characterStatsCurrent = characterStat.Clone();
        thanThongOldUse = null;
        thanThongCoolDown.Clear();
    }
    
    public void HapThuLinhKhi(ELinhKhi elinhKhi , int count)
    {
        switch (elinhKhi)
        {
            case ELinhKhi.None:
                break;
            case ELinhKhi.Kim:
                khiKim += count;
                break;
            case ELinhKhi.Moc:
                khiMoc += count;


                break;
            case ELinhKhi.Thuy:
                khiThuy += count;


                break;
            case ELinhKhi.Hoa:
                khiHoa += count;

                break;
            case ELinhKhi.Tho:
                khiTho += count;

                break;
            case ELinhKhi.Ma:
                khiMa += count;

                break;
            default:
                break;
        }
        Observer.Notify(EventList.GamePlay.HAPTHU_LINHKHI, this,elinhKhi,count );

    }
    public int totalKhi
    {
        get
        {
            int level = characterStatsCurrent.level;

            // 1) Tính đại cảnh giới (3 level = 1 đại cảnh giới)
            int daiCanhGioi = ((level - 1) / 3) + 1;

            // 2) Tính khí mỗi level (base 4 + 1 mỗi đại cảnh giới)
            int baseKhi = 4;
            int khiPerLevel = baseKhi + (daiCanhGioi - 1);

            // 3) Tổng khí
            return khiPerLevel;
        }
    }
    public int totalKhiRandom
    {
        get
        {
            int level = characterStatsCurrent.level;

            // 1) Tính đại cảnh giới (3 level = 1 đại cảnh giới)
            int daiCanhGioi = ((level - 1) / 3) + 1;

            // 2) Tính khí mỗi level (base 9 + 2 mỗi đại cảnh giới)
            int baseKhi = 9;
            int khiPerLevel = baseKhi + (daiCanhGioi - 1) * 2;

            // 3) Tổng khí theo yêu cầu: bạn đang return chỉ số khí/lv
            return khiPerLevel;
        }
    }
    public void TieuTanLinhKhi(ELinhKhi eLinhKhi,int count)
    {
        switch (eLinhKhi)
        {
            case ELinhKhi.None:
                break;
            case ELinhKhi.Kim:
                khiKim -= count; 
                break;
            case ELinhKhi.Moc:
                khiMoc -= count;

                break;
            case ELinhKhi.Thuy:
                khiThuy -= count;

                break;
            case ELinhKhi.Hoa:
                khiHoa -= count;

                break;
            case ELinhKhi.Tho:
                khiTho -= count;

                break;
            case ELinhKhi.Ma:
                khiMa -= count;

                break;
            default:
                break;
        }
    }    
    public void HapThuLinhKhi()
    {
        foreach (var item in HapThuLinhKhi(totalKhiRandom))
        {
            HapThuLinhKhi(item.Key,item.Value);
        }
    }    
    public Dictionary<ELinhKhi,int> HapThuLinhKhi(int total)
    {

        Dictionary<ELinhKhi, int> result = new Dictionary<ELinhKhi, int>();

        // Lấy % từ stats hiện tại
        float pKim = characterStatsCurrent.percentKim;
        float pMoc = characterStatsCurrent.percentMoc;
        float pThuy = characterStatsCurrent.percentThuy;
        float pHoa = characterStatsCurrent.percentHoa;
        float pTho = characterStatsCurrent.percentTho;
        float pMa = characterStatsCurrent.percentMa;

        float totalPercent = pKim + pMoc + pThuy + pHoa + pTho + pMa;

        // Nếu tổng % = 0 (trường hợp hiếm) -> chia đều
        if (totalPercent <= 0)
        {
            pKim = pMoc = pThuy = pHoa = pTho = pMa = 1;
            totalPercent = 6;
        }

        // Tính trọng số normalized
        pKim /= totalPercent;
        pMoc /= totalPercent;
        pThuy /= totalPercent;
        pHoa /= totalPercent;
        pTho /= totalPercent;
        pMa /= totalPercent;

        int khiKim = 0;
        int khiMoc = 0;
        int khiThuy = 0;
        int khiHoa = 0;
        int khiTho = 0;
        int khiMa = 0;

        // Random từng điểm khí
        for (int i = 0; i < total; i++)
        {
            float r = Random.value;

            if (r < pKim)
                khiKim++;
            else if (r < pKim + pMoc)
                khiMoc++;
            else if (r < pKim + pMoc + pThuy)
                khiThuy++;
            else if (r < pKim + pMoc + pThuy + pHoa)
                khiHoa++;
            else if (r < pKim + pMoc + pThuy + pHoa + pTho)
                khiTho++;
            else
                khiMa++;
        }
        // Add vào Dictionary theo enum
        if (khiKim > 0) result.Add(ELinhKhi.Kim, khiKim);
        if (khiMoc > 0) result.Add(ELinhKhi.Moc, khiMoc);
        if (khiThuy > 0) result.Add(ELinhKhi.Thuy, khiThuy);
        if (khiHoa > 0) result.Add(ELinhKhi.Hoa, khiHoa);
        if (khiTho > 0) result.Add(ELinhKhi.Tho, khiTho);
        if (khiMa > 0) result.Add(ELinhKhi.Ma, khiMa);

        return result;
    }
    public void NewTurn()
    {
        var keys = new List<ThanThong>(thanThongCoolDown.Keys);
        foreach (var key in keys)
        {
            AddThanThongCoolDown(key, -1);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SucThe, out int value))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.SucThe, value);
        }
    }
    public bool IsKietSuc()
    {
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KietSuc, out int value))
        {
            if(value > 0)
            {
                return true;
            }    
        }
        return false;

    }
    [Button]   
    public void EndTurn()
    {
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DeTonThuong, out int value))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.DeTonThuong, value);
        }
       
    }
    [Button]
    public void TakeDame(CharacterCombat sender , int dame)
    {
        dame += buffAndDebuffCount.GetValueOrDefault(EBuffAndDeBuff.DeTonThuong, 0);
        int hotrao = buffAndDebuffCount.GetValueOrDefault(EBuffAndDeBuff.HoTrao,0);
        hotrao = Mathf.Clamp(hotrao, 0, dame);
        dame -= hotrao;
        RemoveBuffAndDebuffCount(EBuffAndDeBuff.HoTrao,hotrao);
        RemoveBuffAndDebuffCount(EBuffAndDeBuff.SucThe, dame);

        Debug.Log(this.gameObject.name+ "nhận dame là : " + dame);
        characterStatsCurrent.hp -= dame;
        characterStatsCurrent.hp = Mathf.Clamp(characterStatsCurrent.hp, 0, characterStats.hp);

        Observer.Notify(EventList.GamePlay.CHARACTER_TAKE_DAME);

    }
    public void Healing(CharacterCombat sender, int hp)
    {
        characterStatsCurrent.hp += hp;
        characterStatsCurrent.hp = Mathf.Clamp(characterStatsCurrent.hp, 0, characterStats.hp);


        Observer.Notify(EventList.GamePlay.CHARACTER_TAKE_DAME);

    }

}
