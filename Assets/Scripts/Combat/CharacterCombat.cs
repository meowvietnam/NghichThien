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
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Networking.UnityWebRequest;

[System.Serializable]
public class CharacterStats
{    // chỉ số
    public SecureFloat percentKim;
    public SecureFloat percentMoc;
    public SecureFloat percentThuy;
    public SecureFloat percentHoa;
    public SecureFloat percentTho;
    public SecureFloat percentMa;

    public SecureInt level;
    public SecureFloat hp;
    public string nameUser;
    public SecureInt tuoiTac;
    public SecureInt thoNguyen;
    public SecureInt tuChat;
    public SecureInt thanThuc;
    public SecureInt ngoTinh;
    public SecureInt tocDoBay;
    public SecureInt tamCanh;
    public SecureInt danDoc;
    public SecureInt money;
    public SecureInt idLinhCan;
    // tông môn
    public SecureInt chucVi;
    public SecureInt monPhai;
    // skill
    public SecureInt[] thanThongOwner;
    public SecureInt[] thanThongEquip;
    public class CongPhapSave
    {
        public SecureInt id;
        public SecureInt level;
    }
    public CongPhapSave[] congPhapOwner;
    // công pháp used
    // -1 hoặc là 0 là chưa trang bị
    public SecureInt donThuatUsed;
    public SecureInt phuTu1Used;
    public SecureInt phuTu2Used ;
    public SecureInt phuTu3Used ;
    public SecureInt phuTu4Used ;
    public SecureInt chuTu1Used ;
    public SecureInt chuTu2Used ;
   
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
    public SecureInt allDameTaked;

    public List<LinhKhiTieuTan> linhKhiTieuTanOldPlaySkill = new List<LinhKhiTieuTan>();
    public List<ThanThong> specialThanThongImpactAfterPlaySkill = new List<ThanThong>();
    public List<AmKinhEntry> listAmKinhTT = new List<AmKinhEntry>();
    [System.Serializable]
    public class AmKinhEntry
    {
        public AmKinhTT ThanThong;
        public SecureInt RemainingTurns;

        public AmKinhEntry(AmKinhTT thanThong, SecureInt turns)
        {
            this.ThanThong = thanThong;
            this.RemainingTurns = turns;
        }
    }
    public SecureBool isSkipTieuTanLinhKhi;
    public void AddAmKinhTT(AmKinhTT thanThong , SecureInt countHiep)
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
            if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoaDiem, out SecureInt hoaDiem))
            {
                RemoveBuffAndDebuffCount(EBuffAndDeBuff.HoaDiem, hoaDiem);
            }
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DanLuc, out SecureInt danLuc))
        {
            for (int i = 0; i < danLuc; i++)
            {
                TieuTanLinhKhi(GetRandomLinhKhiConLai(), 1);
            }
        }
        if (BaseSkill.CheckIsEqualETypeSkill(this, new BaseSkill.ETypeSkill[] { BaseSkill.ETypeSkill.Kiem })  && buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemTam, out SecureInt kiemTam))
        {
            AddBuffAndDebuffCount(EBuffAndDeBuff.NguKiem, 1);
            AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, 1);
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.KiemTam, 1);
        }
        StartCoroutine(DelayFrame());
        Debug.Log(this.thanThongCurrentUse.thanThong.name);
        Debug.Log(this.thanThongOldUse?.thanThong.name);
       
        CombatStatic.TrienNhieu(null,this);
      

    }
    private IEnumerator DelayFrame()
    {
        yield return null;
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.AnNac, out SecureInt anNac))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.AnNac, 1);
        }

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
    public SecureInt khiKim;
    public SecureInt khiMoc;
    public SecureInt khiThuy;
    public SecureInt khiHoa;
    public SecureInt khiTho;
    public SecureInt khiMa;

   
    public Dictionary<BaseSkill, SecureInt> skillTichLuy = new Dictionary<BaseSkill, SecureInt>();

    public CharacterStats characterStatsBase;
    public CharacterStats characterStatsMax;
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



    public bool IsSkillTichLuyCanUse(BaseSkill thanThong, SecureInt linhKhiNeedSave , SecureInt require )
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
        public SecureInt count;
    }
    public Dictionary<ThanThong , SecureInt> thanThongCoolDown = new Dictionary<ThanThong, SecureInt>();
    public Dictionary<EBuffAndDeBuff, SecureInt> buffAndDebuffCount = new  Dictionary <EBuffAndDeBuff, SecureInt>();

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

        buffAndDebuffCount.TryGetValue(type, out var current);

        SecureInt newValue = current + count;
        buffAndDebuffCount[type] = newValue;

        Observer.Notify(EventList.GamePlay.ADD_BUFF, this, type, (int)newValue, (int)current);

    }
    [Button]
    public void TryGetCountBuff(EBuffAndDeBuff type)
    {

        SecureInt count = 0;
        buffAndDebuffCount.TryGetValue(type, out count);
        Debug.Log(type + " " + count);


    }
    public void RemoveBuffAndDebuffCount(EBuffAndDeBuff type, int count , Action actionRemoveDone = null)
    {
        if (!buffAndDebuffCount.TryGetValue(type, out SecureInt current))
            return; // không có buff thì khỏi remove

        int newValue = current - count;

        if (newValue <= 0)
        {
            buffAndDebuffCount.Remove(type);
            newValue = 0;
            actionRemoveDone?.Invoke();
        }
        else
        {
            buffAndDebuffCount[type] = newValue;
        }

        Observer.Notify(EventList.GamePlay.ADD_BUFF, this, type, (int)newValue, (int)current);
    }
    [Button]
    public int GetThanThuc()
    {
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThanThuong, out SecureInt thanThuong);
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NgungThan, out SecureInt ngungThan);
        int thanThuc = characterStatsCurrent.thanThuc + ngungThan - thanThuong;
        thanThuc = Mathf.Clamp(thanThuc, 0, int.MaxValue);
        Debug.Log("thanThuc" + thanThuc);
        return thanThuc;

    }    
    public void Init(CharacterStats characterStat)
    {
        characterStatsBase = characterStat;
        characterStatsMax = characterStat.Clone();
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
    public SecureInt totalKhi;

    public SecureInt totalKhiRandom;
    public int CurrentLinhKhi => khiKim + khiMoc+khiThuy+khiHoa+khiTho+khiMa;

    public int baseCountHapThuLinhKhi => CurrentLinhKhi>=totalKhi? totalKhiRandom - totalKhi : totalKhiRandom - CurrentLinhKhi;
    public SecureInt countHapThuLinhKhiNewTurn;

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

        SecureInt treKhi = 0;
        SecureInt tuKhi = 0;

        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TreKhi, out treKhi);
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TuKhi, out tuKhi);

        int totalKhiHapThu = baseCountHapThuLinhKhi + tuKhi - treKhi;
        RemoveBuffAndDebuffCount(EBuffAndDeBuff.TreKhi, treKhi);
        totalKhiHapThu = Mathf.Clamp(totalKhiHapThu,0,int.MaxValue);
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
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SucThe, out SecureInt value))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.SucThe, value);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.AnhNac, out SecureInt anhNac))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.AnhNac, anhNac);
        }
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChanLongChiHuyet,out SecureInt chanLongChiHuyet);
        if(chanLongChiHuyet > 0)
        {
            var debuffKeys = buffAndDebuffCount.Keys
                    .Where(key => BaseSkill.eDebuffs.Contains(key))
                    .ToList();
            // 2. Kiểm tra xem có debuff nào để xóa không
            if (debuffKeys.Count > 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    // 3. Chọn ngẫu nhiên một key trong danh sách debuff đã lọc
                    var randomDebuffKey = debuffKeys[CombatStatic.IndexRandom(0, debuffKeys.Count)];

                    // 4. Thực hiện xóa 1 count
                    RemoveBuffAndDebuffCount(randomDebuffKey, 1);
                }
               
            }

        }
       
        RemoveBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, 1);

        thanThongUseThisComBat.Add(new List<PlaySkillData>());
        
    }
    public bool IsKietSuc()
    {
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KietSuc, out SecureInt kietSuc);
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DongKet, out SecureInt dongKet);

        if (dongKet > 0 || kietSuc > 0)
        {
            return true;
        }
        return false;

    }
    [Button]   
    public void EndTurn()
    {
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChayMau, out SecureInt chayMau))
        {
            TakeDameByStatic(null, new List<int> { (int)(chayMau * 0.01f * (characterStatsMax.hp - characterStatsCurrent.hp)) });
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChayMau, 1);


        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DeTonThuong, out SecureInt value))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.DeTonThuong, value);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.GiamTonThuong, out SecureInt giamTonThuong))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.GiamTonThuong, giamTonThuong);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TriLieu, out SecureInt triLieu))
        {
            Healing(triLieu);
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.TriLieu, 1);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrienNhieu, out SecureInt trienNhieu))
        {

            if (specialThanThongImpactAfterPlaySkill.Any(x => x is TrienTiChu))
            {
                trienNhieu /= 2;
                // end turn giữ lại 1 nữa được xử lí ở character combat
            }

            RemoveBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu, trienNhieu);

        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.PhaThe, out SecureInt phaThe))
        {

            
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.PhaThe, phaThe);

        }
        RemoveBuffAndDebuffCount(EBuffAndDeBuff.TrungDoc, 1);
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoaDiem, out SecureInt hoaDiem);
        if(hoaDiem>0)
        {
            AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, 1);
        }    
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SucLuc, out SecureInt sucLuc))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.SucLuc, sucLuc);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NguKiem, out SecureInt nguKiem))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.NguKiem, nguKiem);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NhanKiem, out SecureInt nhanKiem))
        {
            AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, nhanKiem);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DiaKiem, out SecureInt diaKiem))
        {
            AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, diaKiem);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemTam, out SecureInt kiemTam))
        {
           
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.KiemTam, kiemTam);
        }
        isSkipTieuTanLinhKhi = false;

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
            foreach (var d in dame) Debug.Log("PHÁT HIỆN ADD RANGE GIÁ TRỊ: " + d);

            allDamages.AddRange(dame);
        }
    }

    [Button]
    public void TakeDameByThanThong(CharacterCombat sender, List<int> dames, BaseSkill.ETypeSkill[] eTypeSkils, bool isCongDon = false)
    {

        CaculatorDame(sender, dames, isCongDon, eTypeSkils);
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoaDiem, out SecureInt hoaDiem);
        sender.TakeDameByStatic(this, new List<int> { hoaDiem }, false , new[] { BaseSkill.ETypeSkill.Hoa });
        Observer.Notify(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG, sender, this, dames);

    }
    private void CaculatorDame(CharacterCombat sender, List<int> dames, bool isCongDon = false, BaseSkill.ETypeSkill[] eTypeSkils = null)
    {
        if (sender != null && eTypeSkils != null)
        {
            SecureInt chanKim = 0, chanMoc = 0, chanThuy = 0, chanHoa = 0, chanTho = 0, chanKiem = 0, nguKiem = 0, diem = 0 , chanThan = 0;

            sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SuongGia, out SecureInt suongGia);
            bool isChanKim = sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChanKim, out chanKim)
                             && eTypeSkils.Any(x => x == BaseSkill.ETypeSkill.Kim);

            bool isChanMoc = sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChanMoc, out chanMoc)
                             && eTypeSkils.Any(x => x == BaseSkill.ETypeSkill.Moc);

            bool isChanThuy = sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChanThuy, out chanThuy)
                             && eTypeSkils.Any(x => x == BaseSkill.ETypeSkill.Thuy);

            bool isChanHoa = sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChanHoa, out chanHoa)
                             && eTypeSkils.Any(x => x == BaseSkill.ETypeSkill.Hoa);

            bool isChanTho = sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChanTho, out chanTho)
                             && eTypeSkils.Any(x => x == BaseSkill.ETypeSkill.Tho);

            bool isChanKiem = sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChanKiem, out chanKiem)
                             && eTypeSkils.Any(x => x == BaseSkill.ETypeSkill.Kiem);
            bool isChanThan = sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChanThan, out chanThan)
                           && eTypeSkils.Any(x => x == BaseSkill.ETypeSkill.Than);
            bool isNguKiem = sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NguKiem, out nguKiem)
                             && eTypeSkils.Any(x => x == BaseSkill.ETypeSkill.Kiem);
            bool isDiem = sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Diem, out diem)
                             && eTypeSkils.Any(x => x == BaseSkill.ETypeSkill.Hoa);


            sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SucLuc, out SecureInt sucLuc);
            buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DeTonThuong, out SecureInt deTonThuong);


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
                if (isNguKiem) dame += nguKiem;
                if (isDiem) dame += diem;
                if (isChanThan) dame += chanThan;

                dame += sucLuc;
                dame += deTonThuong;

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
            if (isChanThan) sender.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChanThan, chanThan);

        }

        AddDamageData(dames, isCongDon);
    }
    public void TakeDameByStatic(CharacterCombat sender, List<int> dames,  bool isCongDon = false , BaseSkill.ETypeSkill[] eTypeSkils = null , bool isDef = true)
    {

        CaculatorDame(sender, dames, isCongDon, eTypeSkils);
        TakeDame(sender,isDef);
    }
    [Button]
    public void  TakeDame(CharacterCombat sender , bool isDef)
    {
        void NeTranh()
        {
            SetTextDameAnim("Né Tránh!", Color.whiteSmoke);
            Observer.Notify(EventList.GamePlay.NE_TRANH_SUCCESS, sender, this);
        }
        Observer.Notify(EventList.GamePlay.CHARACTER_BEFORE_TAKE_DAME, sender, this);

        for (int i = 0; i < allDamages.Count; i++)
        {
            if(!isDef)
            {
                TakeDame(sender, allDamages[i], isDef);
                Observer.Notify(EventList.GamePlay.CHARACTER_TAKE_DAME, sender, this, allDamages[i]);
                continue;
            }    
            if (allDamages[i] == 0) continue;
            bool isActiveHuyenVu = CombatStatic.IsActiveHuyenVu(null, this);
            bool isActiveAnNac = CombatStatic.IsActiveAnNac(null, this);
            bool isActiveAnhNac = CombatStatic.IsActiveAnhNac(null, this);

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

                TakeDame(sender, allDamages[i] , isDef);
                Observer.Notify(EventList.GamePlay.CHARACTER_TAKE_DAME, sender, this, allDamages[i]);
            }

        }
       

        allDamages.Clear();


    }
    private void TakeDame(CharacterCombat sender ,int dame , bool isDef)
    {
        if(isDef)
        {
            int giamTonThuong = buffAndDebuffCount.GetValueOrDefault(EBuffAndDeBuff.GiamTonThuong, 0);
            dame -= giamTonThuong;
            int hotraoMax = buffAndDebuffCount.GetValueOrDefault(EBuffAndDeBuff.HoTrao, 0);
            hotraoMax = Mathf.Clamp(hotraoMax, 0, int.MaxValue);
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, dame);
            dame -= hotraoMax;
        }    
        dame = Mathf.Clamp(dame, 0, int.MaxValue);

        RemoveBuffAndDebuffCount(EBuffAndDeBuff.SucThe, dame);

        Debug.Log(this.gameObject.name + "nhận dame là : " + dame);
        
        characterStatsCurrent.hp -= dame;
        characterStatsCurrent.hp = Mathf.Clamp(characterStatsCurrent.hp, 0, characterStatsMax.hp);

        if (dame > 0)
        {
            StartAnimHit();
            SetTextDameAnim($"-{dame}", isDef ? Color.red : Color.purple);
            allDameTaked += dame;

            if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HonLuc, out SecureInt honLuc))
            {
                RemoveBuffAndDebuffCount(EBuffAndDeBuff.HonLuc,dame , () =>
                {
                    if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.QuyLinh, out SecureInt QuyLinh))
                    {
                        RemoveBuffAndDebuffCount(EBuffAndDeBuff.QuyLinh, QuyLinh);



                    }
                    if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.QuyBinh, out SecureInt QuyBinh))
                    {
                        RemoveBuffAndDebuffCount(EBuffAndDeBuff.QuyBinh, QuyBinh);



                    }
                    if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.QuyTuong, out SecureInt QuyTuong))
                    {
                        RemoveBuffAndDebuffCount(EBuffAndDeBuff.QuyTuong, QuyTuong);



                    }
                    if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.QuyTon, out SecureInt quyTon))
                    {
                        RemoveBuffAndDebuffCount(EBuffAndDeBuff.QuyTon, quyTon);



                    }
                    if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.QuyDe, out SecureInt QuyDe))
                    {
                        RemoveBuffAndDebuffCount(EBuffAndDeBuff.QuyDe, QuyDe);



                    }
                });


            }
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
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.PhaThe, out SecureInt phaThe);

        // Tính toán hệ số giảm hồi phục (mỗi điểm phaThe giảm 50%)
        float reductionFactor = Mathf.Pow(0.5f, phaThe);
        int finalHpGain = Mathf.RoundToInt(hp * reductionFactor);

        // Áp dụng vào chỉ số nhân vật
        characterStatsCurrent.hp += finalHpGain;
        characterStatsCurrent.hp = Mathf.Clamp(characterStatsCurrent.hp, 0, characterStatsMax.hp);

        // Hiển thị hiệu ứng số nhảy (Nếu hồi phục > 0 thì mới hiện)
        if (finalHpGain > 0)
        {
            SetTextDameAnim($"+{finalHpGain}", Color.green);
        }


        Observer.Notify(EventList.GamePlay.CHARACTER_ADJUST_HP);

    }

}
