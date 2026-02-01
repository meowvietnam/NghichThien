using AllMaps;
using Base;
using DG.Tweening;
using LongDH.LongDHModules;
using LongDH.Script.ObjectPool;
using NUnit.Framework;
using Sirenix.OdinInspector;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
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

    public List<LinhKhiTieuTan> linhKhiTieuTanOldPlaySkill = new List<LinhKhiTieuTan>();
    public List<ThanThong> specialThanThongImpactAfterPlaySkill = new List<ThanThong>();
    public void AddThanThongImpactAfterPlaySkill(ThanThong thanThong)
    {
        specialThanThongImpactAfterPlaySkill.Add(thanThong);
        // call observer event add UI 

    }
    public void RemoveThanThongImpactAfterPlaySkill(ThanThong thanThong)
    {
        // Lấy type của đối tượng truyền vào
        var typeToRemove = thanThong.GetType();

        // Xóa tất cả các item có cùng Type
        specialThanThongImpactAfterPlaySkill.RemoveAll(item => item.GetType() == typeToRemove);

        // call observer event update UI
    }
    public void PlaySkill(ThanThong thanThong , List<LinhKhiTieuTan> linhKhiTieuTans)
    {
        DOTween.Kill("HideSkillName");

        textSkillNameCall.transform.parent.gameObject.SetActive(true);
        textSkillNameCall.text = thanThong.name;

        DOVirtual.DelayedCall(1f, () =>
        {
            textSkillNameCall.transform.parent.gameObject.SetActive(false);
        }).SetId("HideSkillName");

        StartAnimPunch();
        thanThongCurrentUse = new PlaySkillData { thanThong = thanThong, linhKhiTieuTan=linhKhiTieuTans};
        thanThongUseThisTurn.Add(thanThongCurrentUse);
        thanThongUseThisComBat.Add(thanThongCurrentUse);
        foreach (var item in linhKhiTieuTans)
        {
            switch (item.eLinhKhi)
            {
                case ELinhKhi.None:
                    break;
                case ELinhKhi.Kim:
                    khiKimUsed += item.count;
                    break;
                case ELinhKhi.Moc:
                    khiMocUsed += item.count;

                    break;
                case ELinhKhi.Thuy:
                    khiThuyUsed += item.count;

                    break;
                case ELinhKhi.Hoa:
                    khiHoaUsed += item.count;

                    break;
                case ELinhKhi.Tho:
                    khiThoUsed += item.count;

                    break;
                case ELinhKhi.Ma:
                    khiMaUsed += item.count;

                    break;
                default:
                    break;
            }
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrienNhieu, out int trienNhieu))
        {

            AddDamageData(new List<int> {trienNhieu });
            TakeDame(null);
        }


    }
    public void SetThanThongOldUse(ThanThong thanThong,List<LinhKhiTieuTan> linhKhiTieuTans)
    {
        thanThongOldUse = new PlaySkillData { thanThong = thanThong, linhKhiTieuTan = linhKhiTieuTans };
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
    public void SetTextDameAnim(int dame)
    {
        var textDame = CombatManager.GetTextDame();
        textDame.transform.SetParent(this.transform);
        textDame.transform.localScale = Vector3.one * 2f;

        // 1. Tính toán vị trí dựa trên số lượng text đang hiện có
        // Mỗi text mới sẽ cao hơn text trước đó khoảng 50 đơn vị (tùy chỉnh số này)
        float offset = activeTextCount * 150f;
        Vector3 startPos = new Vector3(0, 1000 + offset, 0);
        textDame.transform.localPosition = startPos;

        activeTextCount++; // Tăng biến đếm khi hiện text mới

        // 2. Thiết lập nội dung và màu sắc
        Color targetColor = (dame > 0) ? Color.green : Color.red;
        textDame.text = dame.ToString();
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

    public int khiKimUsed;
    public int khiMocUsed;
    public int khiThuyUsed;
    public int khiHoaUsed;
    public int khiThoUsed;
    public int khiMaUsed;

    public CharacterStats characterStats;
    public CharacterStats characterStatsCurrent;
    public PlaySkillData thanThongOldUse;
    public PlaySkillData thanThongCurrentUse;
    public List<PlaySkillData> thanThongUseThisTurn = new List<PlaySkillData>();
    public List<PlaySkillData> thanThongUseThisComBat = new List<PlaySkillData>();

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

    public void AddThanThongCoolDown(ThanThong thanThong , int count)
    {
        thanThongCoolDown[thanThong] = thanThongCoolDown.GetValueOrDefault(thanThong, 0) + count;

    }
    [Button]
    public void AddBuffAndDebuffCount(EBuffAndDeBuff type, int count)
    {

        buffAndDebuffCount[type] =
        buffAndDebuffCount.TryGetValue(type, out var current)
        ? current + count
        : count;
        Debug.Log("Vao day3 is Count + type " + count + type);

        Observer.Notify(EventList.GamePlay.ADD_BUFF, this, type, buffAndDebuffCount[type]);


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

        Observer.Notify(EventList.GamePlay.ADD_BUFF, this, type, newValue);
    }
    public void Init(CharacterStats characterStat)
    {
        characterStats = characterStat.Clone();
        characterStatsCurrent = characterStat.Clone();
        thanThongOldUse = null;
        thanThongCoolDown.Clear();
        currentCharGender.gameObject.SetActive(true);
        currentCharGender.AnimationState.SetAnimation(0, ANIM_IDLE, true);
        thanThongUseThisComBat.Clear();
        InitTotalKhi();
        foreach (var cp in congPhapEquip)
        {
            cp.congPhap.Init();
        } 
            

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

    public int BaseCountHapThuLinhKhi => totalKhiRandom - totalKhi;
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
        int khiPerLevelRandom = baseKhiTotalKhiRandom + (daiCanhGioi - 1) * 2;

        // 3) Tổng khí theo yêu cầu: bạn đang return chỉ số khí/lv
        totalKhiRandom = khiPerLevelRandom;
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
        Observer.Notify(EventList.GamePlay.ADJUST_LINHKHI, this, eLinhKhi, count);

    }
    public void HapThuLinhKhiNewTurn(out Dictionary<ELinhKhi, int> dictHapThuLinhKhi)
    {
        int treKhi = 0;
        int tuKhi = 0;

        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TreKhi, out treKhi);
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TuKhi, out tuKhi);

        int totalKhiHapThu = BaseCountHapThuLinhKhi + tuKhi - treKhi;
        RemoveBuffAndDebuffCount(EBuffAndDeBuff.TreKhi, treKhi);
        dictHapThuLinhKhi = DictHapThuLinhKhi(totalKhiHapThu);
        foreach (var item in dictHapThuLinhKhi)
        {
            HapThuLinhKhi(item.Key, item.Value);
        }
        NoiThuongAction();

        RemoveBuffAndDebuffCount(EBuffAndDeBuff.TuKhi, tuKhi);
    }
    public void NoiThuongAction()
    {
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NoiThuong, out int noiThuong))
        {
           
            AddDamageData(new List<int> { noiThuong * BaseCountHapThuLinhKhi });
            TakeDame(null);
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.NoiThuong, 1);
        }
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
        thanThongUseThisTurn.Clear();
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
        TrungDoc();
        RemoveBuffAndDebuffCount(EBuffAndDeBuff.TrungDoc, 1);

    }

    public void TrungDoc()
    {
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrungDoc, out int trungDoc))
        {
            AddDamageData(new List<int> { (int)trungDoc });
            TakeDame(null);

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
    public void TakeDameByThanThong(CharacterCombat sender, List<int> dame, bool isCongDon = true)
    {
        if (sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChanKim, out int chanKim) && BaseSkill.CheckIsEqualETypeSkill(this,new BaseSkill.ETypeSkill[] { BaseSkill.ETypeSkill.Kim }));
        {
            dame[0] += chanKim;
            sender.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChanKim, chanKim);
        }
        if (sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChanMoc, out int chanMoc) && BaseSkill.CheckIsEqualETypeSkill(this, new BaseSkill.ETypeSkill[] { BaseSkill.ETypeSkill.Moc }))
        {
            dame[0] += chanMoc;
            sender.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChanMoc, chanMoc);

        }
        if (sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChanThuy, out int chanThuy) && BaseSkill.CheckIsEqualETypeSkill(this, new BaseSkill.ETypeSkill[] { BaseSkill.ETypeSkill.Thuy }))
        {
            dame[0] += chanThuy;
            sender.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChanThuy, chanThuy);

        }
        if (sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChanHoa, out int chanHoa) && BaseSkill.CheckIsEqualETypeSkill(this, new BaseSkill.ETypeSkill[] { BaseSkill.ETypeSkill.Hoa }))
        {
            dame[0] += chanHoa;
            sender.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChanHoa, chanHoa);

        }
        if (sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChanTho, out int chanTho) && BaseSkill.CheckIsEqualETypeSkill(this, new BaseSkill.ETypeSkill[] { BaseSkill.ETypeSkill.Tho }))
        {
            dame[0] += chanTho;
            sender.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChanTho, chanTho);

        }
        AddDamageData(dame,isCongDon);
        Observer.Notify(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG,sender,this,dame); // sender , targer nhận, dame

    }
    [Button]
    public void TakeDame(CharacterCombat caster)
    {
        foreach (var item in allDamages)
        {
            TakeDame(item);
            Observer.Notify(EventList.GamePlay.CHARACTER_TAKE_DAME, caster,this,item);

        }
        thanThongCurrentUse = null;
        allDamages.Clear();

    }
    private void TakeDame(int dame)
    {
        dame += buffAndDebuffCount.GetValueOrDefault(EBuffAndDeBuff.DeTonThuong, 0);
        int giamTonThuong = buffAndDebuffCount.GetValueOrDefault(EBuffAndDeBuff.GiamTonThuong, 0);
        dame -= giamTonThuong;
        int hotrao = buffAndDebuffCount.GetValueOrDefault(EBuffAndDeBuff.HoTrao, 0);
        hotrao = Mathf.Clamp(hotrao, 0, dame);
        dame -= hotrao;

        dame = Mathf.Clamp(dame, 0, dame);

        RemoveBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hotrao);
        RemoveBuffAndDebuffCount(EBuffAndDeBuff.SucThe, dame);

        Debug.Log(this.gameObject.name + "nhận dame là : " + dame);
        if (dame > 0)
        {
            StartAnimHit();
            SetTextDameAnim(-dame);
        }
        characterStatsCurrent.hp -= dame;
        characterStatsCurrent.hp = Mathf.Clamp(characterStatsCurrent.hp, 0, characterStats.hp);

        Observer.Notify(EventList.GamePlay.CHARACTER_ADJUST_HP);

        CheckDie();
    }

    public virtual void CheckDie()
    {
        if (characterStatsCurrent.hp > 0)
        {
            Die();
        }
    }    
    public virtual void Die()
    {
        StartAnimDie();
    }    
    public void Healing(int hp)
    {
        characterStatsCurrent.hp += hp;
        characterStatsCurrent.hp = Mathf.Clamp(characterStatsCurrent.hp, 0, characterStats.hp);
        SetTextDameAnim(+hp);


        Observer.Notify(EventList.GamePlay.CHARACTER_ADJUST_HP);

    }

}
