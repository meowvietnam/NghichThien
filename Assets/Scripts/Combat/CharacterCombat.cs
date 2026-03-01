using AllMaps;
using Base;
using DG.Tweening;
using LongDH.LongDHModules;
using LongDH.Script.ObjectPool;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Sirenix.OdinInspector;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Networking.UnityWebRequest;

[System.Serializable]
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
        public int level = 1;
    }
    public CongPhapSave[] congPhapOwner;
    // công pháp used
    // -1 là chưa trang bị
    public int donThuatUsed = -1;
    public int phuTu1Used = -1;
    public int phuTu2Used= -1;
    public int phuTu3Used = -1;
    public int phuTu4Used = -1;
    public int chuTu1Used = -1;
    public int chuTu2Used = -1;
   
    public CharacterStats Clone()
    {
        return (CharacterStats)this.MemberwiseClone();
    }



}
public class CharacterCombat : MonoBehaviour
{
    public const string ANIM_IDLE = "Idle_0";
    public const string ANIM_PUNCH = "Punch_0";
    public const string ANIM_HIT = "Hit_0";
    public const string ANIM_DIE = "Die_0";
    public CombatManager CombatManager=>transform.GetComponentInParent<CombatManager>();
    [SerializeField] protected Text textSkillNameCall;
    public Gender gender;
    [SerializeField] SkeletonGraphic[] characterGender;
    [SerializeField] public SkeletonGraphic currentCharGender => characterGender[(int)gender];
    [SerializeField]
    public List<CongPhapMono> congPhapEquip = new List<CongPhapMono>();
    [SerializeField]
    public List<CongPhapMono> congPhapDynamic = new List<CongPhapMono>();
    [SerializeField]
    public int allDameTaked;

    public List<LinhKhiTieuTan> linhKhiTieuTanOldPlaySkill = new List<LinhKhiTieuTan>();
    public List<ThanThong> specialThanThongImpactAfterPlaySkill = new List<ThanThong>();
    public List<AmKinhEntry> listAmKinhTT = new List<AmKinhEntry>();
    [System.Serializable]
    public class AmKinhEntry
    {
        public AmKinhTT ThanThong;
        public int RemainingTurns;

        public AmKinhEntry(AmKinhTT thanThong, int turns)
        {
            this.ThanThong = thanThong;
            this.RemainingTurns = turns;
        }
    }
    public void AddAmKinhTT(AmKinhTT thanThong ,int countHiep)
    {
        if (thanThong != null && thanThong is AmKinhTT) ;
        listAmKinhTT.Add(new AmKinhEntry(thanThong, countHiep));
        // call observer event add UI 

    }
     
    public void AddThanThongImpactAfterPlaySkill(ThanThong thanThong)
    {
        specialThanThongImpactAfterPlaySkill.Add(thanThong);
        // call observer event add UI 

    }
    public void RemoveAllThanThongImpactAfterPlaySkill(ThanThong thanThong)
    {
        // Lấy type của đối tượng truyền vào
        var typeToRemove = thanThong.GetType();

        // Xóa tất cả các item có cùng Type
        specialThanThongImpactAfterPlaySkill.RemoveAll(item => item.GetType() == typeToRemove);

        // call observer event update UI
    }
    public void RemoveOneThanThongImpactAfterPlaySkill(ThanThong thanThong)
    {
        // Lấy type của đối tượng truyền vào
        var typeToRemove = thanThong.GetType();

        // Xóa tất cả các item có cùng Type
        var thanThongRemove = specialThanThongImpactAfterPlaySkill.FirstOrDefault(item => item.GetType() == typeToRemove);
        specialThanThongImpactAfterPlaySkill.Remove(thanThongRemove);
        // call observer event update UI
    }
    public void AddCPDynamic(CongPhapMono cp)
    {
        congPhapDynamic.Add(new CongPhapMono
        {
            congPhap = cp.congPhap,
            level = cp.level,
        });
    }
   
    protected void VisualPlaySkill(ThanThong thanThong)
    {
        DOTween.Kill("HideSkillName");

        textSkillNameCall.transform.parent.gameObject.SetActive(true);
        textSkillNameCall.text = thanThong.name;
        DOVirtual.DelayedCall(thanThong.timeDelayTakeDame <=1 ? 1 : thanThong.timeDelayTakeDame/2, () =>
        {
            textSkillNameCall.transform.parent.gameObject.SetActive(false);
        }).SetId("HideSkillName");
        StartAnimPunch();
    }
    public void PlaySkill(ThanThong thanThong , List<LinhKhiTieuTan> linhKhiTieuTans)
    {
        VisualPlaySkill(thanThong);
         
        var thanThongCurrentUse = new PlaySkillData { thanThong = thanThong, linhKhiTieuTan = linhKhiTieuTans };
        thanThongUseThisComBat[^1].Add(thanThongCurrentUse);
        if (!BaseSkill.CheckIsEqualETypeSkill(this,new BaseSkill.ETypeSkill[] { BaseSkill.ETypeSkill.Hoa }))
        {
            if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoaDiem, out int hoaDiem))
            {
                RemoveBuffAndDebuffCount(EBuffAndDeBuff.HoaDiem, hoaDiem);
            }
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DanLuc, out int danLuc))
        {
            for (int i = 0; i < danLuc; i++)
            {
                TieuTanLinhKhi(GetRandomLinhKhiConLai(), 1);
            }
        }
       
        Debug.Log(this.thanThongCurrentUse.thanThong.name);
        Debug.Log(this.thanThongOldUse?.thanThong.name);
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.AnNac, out int anNac))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.AnNac, anNac);
        }
        CombatStatic.TrienNhieu(null,this);
      

    }
   
    public void StartAnimPunch()
    {
        // Chạy anim tấn công ngay lập tức
        currentCharGender.AnimationState.SetAnimation(0, ANIM_PUNCH, false);
        // Xếp hàng anim Idle để chạy ngay sau khi anim trên kết thúc
        currentCharGender.AnimationState.AddAnimation(0, ANIM_IDLE, true, 0);
    }

    public void StartAnimHit()
    {
        // Reset toàn bộ xương về tư thế gốc trước khi diễn
        currentCharGender.Skeleton.SetToSetupPose();
        currentCharGender.AnimationState.ClearTracks();

        // Chạy Hit
        currentCharGender.AnimationState.SetAnimation(0, ANIM_HIT, false);

        // Dùng Coroutine để ép về Idle sau đúng thời gian của anim Hit
        StartCoroutine(ForceReturnToIdle());
    }

    IEnumerator ForceReturnToIdle()
    {
        var anim = currentCharGender.Skeleton.Data.FindAnimation(ANIM_HIT);
        // Lấy thời gian thực tế trừ đi khoảng 0.1s hoặc 0.2s để chuyển cảnh sớm hơn
        float duration = (anim != null) ? anim.Duration : 0.1f;
        float spacing = gender == Gender.Female ? 1.75f : 0;
        // Giảm thời gian chờ đi một chút để bớt cảm giác "đơ"
        float actualWait = Mathf.Max(0, duration - spacing);

        yield return new WaitForSeconds(actualWait);

        // Ép về Idle
        currentCharGender.AnimationState.SetAnimation(0, ANIM_IDLE, true);
    }
    public void StartAnimDie()
    {
        // Chạy anim tấn công ngay lập tức
        currentCharGender.AnimationState.SetAnimation(0, ANIM_DIE, false);
        
    }
    private int activeTextCount = 0; // Đếm số text đang bay
    public void SetTextDameAnim(string content,Color color)
    {
        var textDame = CombatManager.GetTextDame();
        textDame.transform.SetParent(CombatManager.parentUIFight);
        textDame.transform.localScale = Vector3.one * 0.75f ;
        textDame.transform.position = this.transform.position;
        // 1. Tính toán vị trí dựa trên số lượng text đang hiện có
        // Mỗi text mới sẽ cao hơn text trước đó khoảng 50 đơn vị (tùy chỉnh số này)
        float offset = activeTextCount * 75f;
        Vector3 startPos = new Vector3(0, 350 + offset, 0);
        textDame.transform.localPosition += startPos;

        activeTextCount++; // Tăng biến đếm khi hiện text mới

        // 2. Thiết lập nội dung và màu sắc
        Color targetColor = color;
        textDame.text = content;
        targetColor.a = 1;
        textDame.color = targetColor;

        float time = 1f;
        float flyHeight = 200;

        // 3. Hiệu ứng
        textDame.transform.DOLocalMoveY(textDame.transform.localPosition.y + flyHeight, time);
        textDame.DOFade(0, time).OnComplete(() => {
            activeTextCount--; // Giảm biến đếm khi text biến mất
            if (activeTextCount < 0) activeTextCount = 0; // Safety check
            textDame.gameObject.Recycle();
        });
    }
    public int khiKim;
    public int khiMoc;
    public int khiThuy;
    public int khiHoa;
    public int khiTho;
    public int khiMa;

   
    public Dictionary<BaseSkill, int> skillTichLuy = new Dictionary<BaseSkill, int>();

    public CharacterStats characterStats;
    public CharacterStats characterStatsCurrent;
    public List<PlaySkillData> thanThongUseThisTurn =>
        thanThongUseThisComBat.Count > 0 ? thanThongUseThisComBat[^1] : null;
    public List<PlaySkillData> thanThongUseLastTurn =>
        thanThongUseThisComBat.Count > 1 ? thanThongUseThisComBat[^2] : null;
    public List<List<PlaySkillData>> thanThongUseThisComBat = new List<List<PlaySkillData>>();
    public PlaySkillData thanThongCurrentUse
    {
        get
        {
            List<PlaySkillData> thanThanTurnCheck = thanThongUseThisComBat
                               .SelectMany(t => t)
                                .ToList();
            if (thanThanTurnCheck == null || thanThanTurnCheck.Count <= 0)
                return null;

            return thanThanTurnCheck[^1];
        }
    }

    public PlaySkillData thanThongOldUse
    {
        get
        {
            List<PlaySkillData> thanThanTurnCheck = thanThongUseThisComBat
                                .SelectMany(t => t)
                                 .ToList();
            if (thanThanTurnCheck == null || thanThanTurnCheck.Count <= 1) 
                return null;

          

            return thanThanTurnCheck[^2];
        }
    }



    public bool IsSkillTichLuyCanUse(BaseSkill thanThong, int linhKhiNeedSave , int require )
    {
        // 1. Tìm chính xác cái Key đang nằm trong Dictionary dựa trên điều kiện của bạn
        var key = skillTichLuy.Keys.FirstOrDefault(x =>
                    x != null &&
                    x.GetType() == thanThong.GetType() &&
                    x.eTypeSkill == thanThong.eTypeSkill
                  );

        // 2. Nếu không tìm thấy Key phù hợp, trả về false ngay lập tức
        if (key == null)
        {
            Debug.Log("key null");
            skillTichLuy.Add(thanThong, linhKhiNeedSave);
            return false;
        }
        skillTichLuy[key] += linhKhiNeedSave;

        // 3. Kiểm tra số lượng tích lũy
        if (skillTichLuy[key] >= require)
        {
            // Trừ trực tiếp vào giá trị của Dictionary thông qua Key đã tìm thấy
            skillTichLuy[key] -= require;
            return true;
        }

        // 4. Nếu không đủ: Không cộng thêm (bỏ logic item.Value += require cũ)
        return false;
    }
    [Serializable]
    public class PlaySkillData
    {
        public ThanThong thanThong;
        public List<LinhKhiTieuTan> linhKhiTieuTan;
    }
    public class CountStack<T>
    {
        public T type;
        public int count;
    }
    public Dictionary<ThanThong , int> thanThongCoolDown = new Dictionary<ThanThong,int>();
    public Dictionary<EBuffAndDeBuff, int> buffAndDebuffCount = new  Dictionary <EBuffAndDeBuff, int>();

    public void AddThanThongCoolDown(ThanThong thanThong, int count)
    {
        int current = thanThongCoolDown.GetValueOrDefault(thanThong, 0);
        int newValue = current + count;

        if (newValue <= 0)
            thanThongCoolDown.Remove(thanThong);
        else
            thanThongCoolDown[thanThong] = newValue;
    }
    [Button]
    public void AddBuffAndDebuffCount(EBuffAndDeBuff type, int count)
    {

        buffAndDebuffCount[type] =
        buffAndDebuffCount.TryGetValue(type, out var current)
        ? current + count
        : count;
        Debug.Log("Vao day3 is Count + type " + count + type);

        Observer.Notify(EventList.GamePlay.ADD_BUFF, this, type, buffAndDebuffCount[type], current);


    }
    [Button]
    public void TryGetCountBuff(EBuffAndDeBuff type)
    {

        int count = 0;
        buffAndDebuffCount.TryGetValue(type, out count);
        Debug.Log(type + " " + count);


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

        Observer.Notify(EventList.GamePlay.ADD_BUFF, this, type, newValue , current);
    }
    public void Init(CharacterStats characterStat)
    {
        characterStats = characterStat.Clone();
        characterStatsCurrent = characterStat.Clone();
        thanThongCoolDown.Clear();
        currentCharGender.gameObject.SetActive(true);
        currentCharGender.AnimationState.SetAnimation(0, ANIM_IDLE, true);
        thanThongUseThisComBat.Clear();
        InitTotalKhi();
        congPhapDynamic.Clear();
        skillTichLuy.Clear();
        foreach (var cp in congPhapEquip)
        {
            AddCPDynamic(cp);
            cp.congPhap.InitPosCallExecute(this);
        }
        Debug.Log("dcmmm " + congPhapDynamic.Count);



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
        Observer.Notify(EventList.GamePlay.ADJUST_LINHKHI, this,elinhKhi,count );

    }
    public int totalKhi;

    public int totalKhiRandom;
    public int CurrentLinhKhi => khiKim + khiMoc+khiThuy+khiHoa+khiTho+khiMa;

    public int baseCountHapThuLinhKhi => CurrentLinhKhi>=totalKhi? totalKhiRandom - totalKhi : totalKhiRandom - CurrentLinhKhi;
    public int countHapThuLinhKhiNewTurn;

    private void InitTotalKhi()
    {
        int level = characterStatsCurrent.level;

        // 1) Tính đại cảnh giới (3 level = 1 đại cảnh giới)
        int daiCanhGioi = ((level - 1) / 3) + 1;

        // 2) Tính khí mỗi level (base 4 + 1 mỗi đại cảnh giới)
        int baseKhi = 4;
        int khiPerLevel = baseKhi + (daiCanhGioi - 1);

        // 3) Tổng khí
        totalKhi = khiPerLevel;

     
        int baseKhiTotalKhiRandom = 9;
        int khiPerLevelRandom = baseKhiTotalKhiRandom + 2*(daiCanhGioi - 1);

        // 3) Tổng khí theo yêu cầu: bạn đang return chỉ số khí/lv
        totalKhiRandom = khiPerLevelRandom;

    }
    [Button]    
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
        Observer.Notify(EventList.GamePlay.ADJUST_LINHKHI, this, eLinhKhi, -count);

    }
    public void TieuTanAllLinhKhi()
    {
        TieuTanLinhKhi(ELinhKhi.Kim,khiKim);
        TieuTanLinhKhi(ELinhKhi.Moc, khiMoc);
        TieuTanLinhKhi(ELinhKhi.Thuy, khiThuy);
        TieuTanLinhKhi(ELinhKhi.Hoa, khiHoa);
        TieuTanLinhKhi(ELinhKhi.Tho, khiTho);
        TieuTanLinhKhi(ELinhKhi.Ma, khiMa);

    }
    public int GetLinhKhi(ELinhKhi e)
    {
        switch (e)
        {
            case ELinhKhi.Kim: return khiKim;
            case ELinhKhi.Moc: return khiMoc;
            case ELinhKhi.Thuy: return khiThuy;
            case ELinhKhi.Hoa: return khiHoa;
            case ELinhKhi.Tho: return khiTho;
            case ELinhKhi.Ma: return khiMa;
            default: return 0;
        }
    }
    public ELinhKhi GetRandomLinhKhiConLai()
    {
        List<ELinhKhi> list = new List<ELinhKhi>();

        foreach (ELinhKhi e in Enum.GetValues(typeof(ELinhKhi)))
        {
            if (e == ELinhKhi.None) continue;

            if (GetLinhKhi(e) > 0)
                list.Add(e);
        }

        if (list.Count == 0)
            return ELinhKhi.None;

        int index = CombatStatic.IndexRandom(0,list.Count);
        return list[index];
    }
    public void HapThuLinhKhiNewTurn(out Dictionary<ELinhKhi, int> dictHapThuLinhKhi)
    {
           
        int treKhi = 0;
        int tuKhi = 0;

        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TreKhi, out treKhi);
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TuKhi, out tuKhi);

        int totalKhiHapThu = baseCountHapThuLinhKhi + tuKhi - treKhi;
        RemoveBuffAndDebuffCount(EBuffAndDeBuff.TreKhi, treKhi);
        dictHapThuLinhKhi = DictHapThuLinhKhi(totalKhiHapThu);
        foreach (var item in dictHapThuLinhKhi)
        {
            HapThuLinhKhi(item.Key, item.Value);
        }
        NoiThuongAction();
        RemoveBuffAndDebuffCount(EBuffAndDeBuff.NoiThuong, 1);
        RemoveBuffAndDebuffCount(EBuffAndDeBuff.TuKhi, tuKhi);
    }
    public void NoiThuongAction()
    {
        CombatStatic.NoiThuong(null, this);

    }


    public void HapThuLinhKhi(int total)
    {
        foreach (var item in DictHapThuLinhKhi(total))
        {
            HapThuLinhKhi(item.Key, item.Value);
        }
    }
    public Dictionary<ELinhKhi,int> DictHapThuLinhKhi(int total)
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
            float r = UnityEngine.Random.value;

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
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SucLuc, out int sucLuc))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.SucLuc, sucLuc);
        }
        RemoveBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, 1);

        thanThongUseThisComBat.Add(new List<PlaySkillData>());
        
    }
    public bool IsKietSuc()
    {
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KietSuc, out int kietSuc);
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DongKet, out int dongKet);

        if (dongKet > 0 || kietSuc > 0)
        {
            return true;
        }
        return false;

    }
    [Button]   
    public void EndTurn()
    {
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChayMau, out int chayMau))
        {
            AddDamageData(new List<int> { (int)(chayMau * 0.01f * (characterStats.hp - characterStatsCurrent.hp)) });
            TakeDame(null);
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChayMau, 1);


        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DeTonThuong, out int value))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.DeTonThuong, value);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.GiamTonThuong, out int giamTonThuong))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.GiamTonThuong, giamTonThuong);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TriLieu, out int triLieu))
        {
            Healing(triLieu);
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.TriLieu, 1);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrienNhieu, out int trienNhieu))
        {

            if (specialThanThongImpactAfterPlaySkill.Any(x => x is TrienTiChu))
            {
                trienNhieu /= 2;
                // end turn giữ lại 1 nữa được xử lí ở character combat
            }

            RemoveBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu, trienNhieu);

        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.PhaThe, out int phaThe))
        {

            
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.PhaThe, phaThe);

        }
        RemoveBuffAndDebuffCount(EBuffAndDeBuff.TrungDoc, 1);
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoaDiem, out int hoaDiem);
        if(hoaDiem>0)
        {
            AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, 1);
        }    
    }

  

    public List<int> allDamages = new List<int>();

    public void AddDamageData(List<int> dame, bool isCongDon = true)
    {
        if (isCongDon)
        {
            int sum = 0;
            foreach (var d in dame)
            {
                sum += d;
            }
            if(allDamages.Count>0)
            {
                for (int i = 0; i < allDamages.Count; i++)
                {
                    allDamages[i] += sum;
                }
            }   
            else
            {
                allDamages.Add(sum);
            }    

        }
        else
        {
            // Nếu không cộng dồn, luôn luôn thêm một phần tử mới vào List
            // Việc này giúp lưu lại từng đòn đánh riêng biệt (không ghi đè)
            allDamages.AddRange(dame);
        }
    }

    [Button]
    public void TakeDameByThanThong(CharacterCombat sender, List<int> dames, bool isCongDon = false)
    {
        int chanKim = 0, chanMoc = 0, chanThuy = 0, chanHoa = 0, chanTho = 0, chanKiem = 0;

        // ở đây là thằng nhận dame => lấy sương giá của sender
        sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SuongGia, out int suongGia);
        bool isChanKim = sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChanKim, out chanKim)
                         && BaseSkill.CheckIsEqualETypeSkill(sender, new[] { BaseSkill.ETypeSkill.Kim });

        bool isChanMoc = sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChanMoc, out chanMoc)
                         && BaseSkill.CheckIsEqualETypeSkill(sender, new[] { BaseSkill.ETypeSkill.Moc });

        bool isChanThuy = sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChanThuy, out chanThuy)
                          && BaseSkill.CheckIsEqualETypeSkill(sender, new[] { BaseSkill.ETypeSkill.Thuy });

        bool isChanHoa = sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChanHoa, out chanHoa)
                         && BaseSkill.CheckIsEqualETypeSkill(sender, new[] { BaseSkill.ETypeSkill.Hoa });

        bool isChanTho = sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChanTho, out chanTho)
                         && BaseSkill.CheckIsEqualETypeSkill(sender, new[] { BaseSkill.ETypeSkill.Tho });

        bool isChanKiem = sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChanKiem, out chanKiem)
                          && BaseSkill.CheckIsEqualETypeSkill(sender, new[] { BaseSkill.ETypeSkill.Kiem });

        sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SucLuc, out int sucLuc);
        sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Diem, out int diem);

        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoaDiem, out int hoaDiem);
        sender.AddDamageData(new List<int> { hoaDiem}, false);
        sender.TakeDame(this);

        // ===== XỬ LÝ TỪNG DAME =====
        for (int i = 0; i < dames.Count; i++)
        {
            int dame = dames[i];

            if (isChanKim) dame += chanKim;
            if (isChanMoc) dame += chanMoc;
            if (isChanThuy) dame += chanThuy;
            if (isChanHoa) dame += chanHoa;
            if (isChanTho) dame += chanTho;
            if (isChanKiem) dame += chanKiem;
            dame += sucLuc;
            if (BaseSkill.CheckIsEqualETypeSkill(sender, new BaseSkill.ETypeSkill[] { BaseSkill.ETypeSkill.Hoa }))
            {
                dame += diem;

            }
               

            dame -= suongGia;

            dame = Mathf.Clamp(dame, 0, int.MaxValue);

            dames[i] = dame;
        }

        // ===== REMOVE BUFF SAU KHI DÙNG =====
        sender.RemoveBuffAndDebuffCount(EBuffAndDeBuff.SuongGia, 1);

        if (isChanKim) sender.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChanKim, chanKim);
        if (isChanMoc) sender.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChanMoc, chanMoc);
        if (isChanThuy) sender.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChanThuy, chanThuy);
        if (isChanHoa) sender.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChanHoa, chanHoa);
        if (isChanTho) sender.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChanTho, chanTho);
        if (isChanKiem) sender.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChanKiem, chanKiem);

        AddDamageData(dames, isCongDon);

        Observer.Notify(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG, sender, this, dames);

    }
    [Button]
    public void  TakeDame(CharacterCombat sender)
    {
        void NeTranh()
        {
            SetTextDameAnim("Né Tránh!", Color.whiteSmoke);
            Observer.Notify(EventList.GamePlay.NE_TRANH_SUCCESS, sender, this);
        }

        for (int i = 0; i < allDamages.Count; i++)
        {
            if (allDamages[i] == 0) continue;
            bool isActiveHuyenVu = CombatStatic.IsActiveHuyenVu(null, this);
            bool isActiveAnNac = CombatStatic.IsActiveAnNac(null, this);
            if (isActiveHuyenVu || isActiveAnNac)
            {
                allDamages[i] = -1;
            }
            
            Observer.Notify(EventList.GamePlay.CHECK_NE_TRANH, sender, this, i);
            var dame = allDamages[i];
            bool isNeTranh = dame == -1;


            
            if (isNeTranh)
            {
                NeTranh();

               
            }
            else
            {


                TakeDame(sender, dame);
                Observer.Notify(EventList.GamePlay.CHARACTER_TAKE_DAME, sender, this, dame);
            }

        }
       

        allDamages.Clear();

    }
    private void TakeDame(CharacterCombat sender ,int dame)
    {
        dame += buffAndDebuffCount.GetValueOrDefault(EBuffAndDeBuff.DeTonThuong, 0);
        int giamTonThuong = buffAndDebuffCount.GetValueOrDefault(EBuffAndDeBuff.GiamTonThuong, 0);
        dame -= giamTonThuong;
        int hotraoMax = buffAndDebuffCount.GetValueOrDefault(EBuffAndDeBuff.HoTrao, 0);
        hotraoMax = Mathf.Clamp(hotraoMax, 0, int.MaxValue);
        RemoveBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, dame);
        dame -= hotraoMax;
        dame = Mathf.Clamp(dame, 0, int.MaxValue);

        RemoveBuffAndDebuffCount(EBuffAndDeBuff.SucThe, dame);

        Debug.Log(this.gameObject.name + "nhận dame là : " + dame);
        
        characterStatsCurrent.hp -= dame;
        characterStatsCurrent.hp = Mathf.Clamp(characterStatsCurrent.hp, 0, characterStats.hp);

        if (dame > 0)
        {
            StartAnimHit();
            SetTextDameAnim($"-{dame}", Color.red);
            allDameTaked += dame;
        }
        Observer.Notify(EventList.GamePlay.CHARACTER_ADJUST_HP);
        

    }
    public void SetHp(int hp)
    {
        characterStatsCurrent.hp = hp;
        Observer.Notify(EventList.GamePlay.CHARACTER_ADJUST_HP);

    }

    public virtual bool CheckDie()
    {
        if (characterStatsCurrent.hp > 0)
        {
            Die();
            return true;
        }
        return false;
    }    
    public virtual void Die()
    {
        StartAnimDie();
    }    
    public void Healing(int hp)
    {
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.PhaThe, out int phaThe);

        // Tính toán hệ số giảm hồi phục (mỗi điểm phaThe giảm 50%)
        float reductionFactor = Mathf.Pow(0.5f, phaThe);
        int finalHpGain = Mathf.RoundToInt(hp * reductionFactor);

        // Áp dụng vào chỉ số nhân vật
        characterStatsCurrent.hp += finalHpGain;
        characterStatsCurrent.hp = Mathf.Clamp(characterStatsCurrent.hp, 0, characterStats.hp);

        // Hiển thị hiệu ứng số nhảy (Nếu hồi phục > 0 thì mới hiện)
        if (finalHpGain > 0)
        {
            SetTextDameAnim($"+{finalHpGain}", Color.green);
        }


        Observer.Notify(EventList.GamePlay.CHARACTER_ADJUST_HP);

    }

}
