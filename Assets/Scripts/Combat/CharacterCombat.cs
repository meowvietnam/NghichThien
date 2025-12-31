using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


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
    public List<CountStack<ThanThong>> thanThongCoolDown = new List<CountStack<ThanThong>>();


    public void Init(CharacterStats characterStat)
    {
        characterStats = characterStat;
        characterStatsCurrent = characterStat;
        thanThongOldUse = null;
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

        int total = totalKhiRandom;

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
    }
    public void TakeDame(CharacterCombat sender , int dame)
    {
        Debug.Log(this.gameObject.name+ "nhận dame là : " + dame);
        characterStatsCurrent.hp -= dame;
    }


}
