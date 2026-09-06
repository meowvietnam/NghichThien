 using AllMaps;
using Base;
using CreateCharacter;
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
using Combat;
using LongDHModules;
using Skill;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Networking.UnityWebRequest;
using CongPhap = Skill.CongPhap;
using ThanThong = Skill.ThanThong;
using Network;
using Network.Models;

public enum ETypeVisualChar
{
    Character,

    EnemyLuyenKhiSoKi,
    EnemyLuyenKhiTrungKi ,
    EnemyLuyenKhiHauKi ,

    EnemyTrucCoSoKi ,
    EnemyTrucCoTrungKi,
    EnemyTrucCoHauKi,

    EnemyKetDanSoKi,
    EnemyKetDanTrungKi,
    EnemyKetDanHauKi,

    EnemyNguyenAnhSoKi,
    EnemyNguyenAnhTrungKi,
    EnemyNguyenAnhHauKi,

    EnemyStory0_1,
    EnemyStory0_2,
    EnemyStory0_3,
    EnemyStory1_1,
    EnemyStory1_2,
    EnemyStory2,
    EnemyStory3_1,
    EnemyStory3_2,
    EnemyStory4,
    EnemyStory5,
    EnemyStory6,
    EnemyStory7,
    EnemyStory8,
    EnemyStory9_1,
}
public class CharacterCombat : MonoBehaviour
{
    public CombatManager CombatManager=>transform.GetComponentInParent<CombatManager>();
    [SerializeField] protected Text textSkillNameCall;
    [SerializeField] public CharacterVisualControl characterVisualControl;
    [SerializeField] public SkeletonGraphic currentCharGender => characterVisualControl.currentCharGender;
    [SerializeField]
    public List<CongPhapMono> congPhapEquip = new List<CongPhapMono>();
    [SerializeField]
    public List<ThanThong> thanThongEquip = new List<ThanThong>();
    [SerializeField]
    public List<CongPhapMono> congPhapDynamic = new List<CongPhapMono>();
    [SerializeField]
    public SecureInt allDameTaked => listDameTaked.Sum(x=>x.Value);
    [SerializeField]
    public SecureInt currentTurnDameTaked => listDameTaked.Count > 0 ? listDameTaked[^1] : null;
    public SecureInt oldTurnDameTaked => listDameTaked.Count > 1 ? listDameTaked[^2] : null;
    private IPlayerCombat playerCombat;
    [SerializeField]
    public List<SecureInt> listDameTaked;

    [SerializeField]
    public List<BaseSkill> skillVoHieuHoa;
    public void AddSkillVoHieuHoa(BaseSkill baseSkill)
    {
        skillVoHieuHoa.Add(baseSkill);
    }
    public int CurrentTurn => thanThongUseThisComBat.Count;
    public List<LinhKhiTieuTan> linhKhiTieuTanOldPlaySkill = new List<LinhKhiTieuTan>();
    public List<ThanThong> specialThanThongImpactAfterPlaySkill = new List<ThanThong>();
    public List<NgoDaoCombat> ngoDaoCombats = new List<NgoDaoCombat>();
    public List<NgoDaoCombat> ngoDaoCombatsDynamic = new List<NgoDaoCombat>();

    public List<CountStack<BaseDanDuocCombat>> danDuocCombats = new List<CountStack<BaseDanDuocCombat>>();
    public List<BaseDanDuocCombat> danDuocCombatsDynamic = new List<BaseDanDuocCombat>();

    public List<AmKinhEntry> listAmKinhTT = new List<AmKinhEntry>();
    public SecureBool isAutoCastSkill;
    public Dictionary<BaseSkill,GameObject> effectSkillNoDestroy = new Dictionary<BaseSkill,GameObject>();
    public void RemoveEffectSkillNoDestroy(BaseSkill skill)
    {
        effectSkillNoDestroy.TryGetValue(skill, out var effect);
        if (effect != null)
        {
            effect.Recycle();
        }
        effectSkillNoDestroy.Remove(skill);
    }    
 
    public SecureBool isCooldownWeapon;
    public List<PhapBaoVisual> vuKhiVisuals;
    public PhapBaoVisual currentVuKhiVisual
    {
        get
        {
            // 1. Kiểm tra các biến gốc có tồn tại không
            if (vuKhiVisuals == null || characterStatsBase?.vuKhi?.Item == null)
                return null;

            // 2. Lấy Type cần tìm
            var targetType = characterStatsBase.vuKhi.Item.Type;

            // 3. Tìm kiếm và lọc bỏ những phần tử null trong list (nếu có)
            return vuKhiVisuals.FirstOrDefault(x => x != null && x.artifactType == targetType);
        }
    }

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
        if (thanThong != null && thanThong != null)
        {
            
        }
        AmKinhEntry amKinh = new AmKinhEntry(thanThong, countHiep);
        listAmKinhTT.Add(amKinh);
        Observer.Notify(EventList.Combat.UPDATE_AMKINH_THANTHONG,this, amKinh);

        // call observer event add UI 

    }

    public void AddThanThongImpactAfterPlaySkill(ThanThong thanThong)
    {
        int currentValue = specialThanThongImpactAfterPlaySkill.Count(x => x == thanThong);
        specialThanThongImpactAfterPlaySkill.Add(thanThong);
        int newValue = currentValue+1;

        Observer.Notify(EventList.Combat.ADD_SKILL,this, thanThong, newValue,currentValue , ThanThong.GetDaiCanhGioi(this));

        // call observer event add UI 

    }
    public void RemoveAllThanThongImpactAfterPlaySkill(ThanThong thanThong)
    {
        // Lấy type của đối tượng truyền vào
        var typeToRemove = thanThong.Id;
        int currentValue = specialThanThongImpactAfterPlaySkill.Count(item => item.Id == typeToRemove);
        // Xóa tất cả các item có cùng Type
        specialThanThongImpactAfterPlaySkill.RemoveAll(item => item.Id == typeToRemove);
        Observer.Notify(EventList.Combat.ADD_SKILL,this, thanThong, 0, currentValue, ThanThong.GetDaiCanhGioi(this));

        // call observer event update UI
    }
    public void RemoveOneThanThongImpactAfterPlaySkill(ThanThong thanThong)
    {
        // Lấy type của đối tượng truyền vào
        var typeToRemove = thanThong.Id;
        int currentValue = specialThanThongImpactAfterPlaySkill.Count(x => x == thanThong);

        // Xóa tất cả các item có cùng Type
        var thanThongRemove = specialThanThongImpactAfterPlaySkill.FirstOrDefault(item => item.Id == typeToRemove);
        specialThanThongImpactAfterPlaySkill.Remove(thanThongRemove);
        int newValue = currentValue - 1;

        Observer.Notify(EventList.Combat.ADD_SKILL,this, thanThong, newValue,currentValue, ThanThong.GetDaiCanhGioi(this));

        // call observer event update UI
    }
    public void AddCPDynamic(CongPhapMono cp , int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            int currentValue = congPhapDynamic.Count(x => x.congPhap == cp.congPhap);
            congPhapDynamic.Add(cp);
            int newValue = currentValue + 1;
            Observer.Notify(EventList.Combat.ADD_SKILL, this, cp.congPhap, newValue, currentValue, cp.level.Value);

        }

    }
    public void RemoveCPDynamic(CongPhap cp)
    {
        int currentValue = congPhapDynamic.Count(x => x.congPhap == cp);

        congPhapDynamic.RemoveAll(x =>
            x.congPhap == cp
        );
        Observer.Notify(EventList.Combat.ADD_SKILL, this, cp, 0, currentValue, 1);

       

    }
    public void RemoveDanDuocDynamic(BaseDanDuocCombat dd)
    {
        danDuocCombatsDynamic.RemoveAll(x =>
           x == dd
        );
    }    
    public void AddNgoDaoCombatDynamic(NgoDaoCombat nd, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            ngoDaoCombatsDynamic.Add(nd);
           

        }

    }
    public void RemoveNgoDaoCombatDynamic(NgoDaoCombat nd)
    {
        ngoDaoCombatsDynamic.RemoveAll(x =>
           x == nd
       );

    }

    public void VisualPlaySkill(ThanThong thanThong)
    {
        playerCombat?.PlaySkill(thanThong);
        DOTween.Kill("HideSkillName");

        textSkillNameCall.transform.parent.gameObject.SetActive(true);
        textSkillNameCall.text = thanThong.nameItem;
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
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TichMoc, out SecureInt TichMoc))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu, TichMoc);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TichThuy, out SecureInt TichThuy))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.SuongGia, TichThuy);
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
            AddBuffAndDebuffCount(EBuffAndDeBuff.NguKiemHiep, 1);
            AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, 1);
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.KiemTam, 1);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.AnNac, out SecureInt anNac))
        {
           if(!(thanThong is ThucThanThu || thanThong is ThuHonThuat || thanThong is TinhThanThanNiemThe))
           {
                RemoveBuffAndDebuffCount(EBuffAndDeBuff.AnNac, 1);
           }    

        }

       // StartCoroutine(DelayFrame());
        Debug.Log(this.thanThongCurrentUse.thanThong.name);
        Debug.Log(this.thanThongOldUse?.thanThong.name);
       
      

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
        characterVisualControl.AnimPunch();
        // Xếp hàng anim Idle để chạy ngay sau khi anim trên kết thúc
        currentCharGender.AnimationState.AddAnimation(0, characterVisualControl.ANIM_IDLE, true, 0);
    }

    public void StartAnimHit()
    {
        // Reset toàn bộ xương về tư thế gốc trước khi diễn
        currentCharGender.Skeleton.SetToSetupPose();
        currentCharGender.AnimationState.ClearTracks();

        // Chạy Hit
        characterVisualControl.AnimHit();
        // Dùng Coroutine để ép về Idle sau đúng thời gian của anim Hit
        StartCoroutine(ForceReturnToIdle());
    }

    IEnumerator ForceReturnToIdle()
    {
        var anim = currentCharGender.Skeleton.Data.FindAnimation(characterVisualControl.ANIM_HIT);
        // Lấy thời gian thực tế trừ đi khoảng 0.1s hoặc 0.2s để chuyển cảnh sớm hơn
        float duration = (anim != null) ? anim.Duration : 0.1f;
        float spacing = characterStatsBase.Gender == Gender.Female ? 1.75f : 0;
        // Giảm thời gian chờ đi một chút để bớt cảm giác "đơ"
        float actualWait = Mathf.Max(0, duration - spacing);

        yield return new WaitForSeconds(actualWait);

        // Ép về Idle
        characterVisualControl.AnimIdle();
    }
    public void StartAnimDie()
    {
        // Chạy anim tấn công ngay lập tức
        characterVisualControl.AnimDie();
        
        
    }
    // Khai báo struct để lưu trữ thông tin text chờ hiển thị
    public struct DamageQueueData
    {
        public string content;
        public Color color;
    }

    private Queue<DamageQueueData> damageQueue = new Queue<DamageQueueData>();
    private bool isProcessingQueue = false;
    private int activeTextCount = 0;

    public void SetTextDameAnim(string content, Color color)
    {
        // Thêm dữ liệu vào hàng đợi
        damageQueue.Enqueue(new DamageQueueData { content = content, color = color });

        // Nếu chưa có tiến trình nào chạy thì bắt đầu xử lý hàng đợi
        if (!isProcessingQueue)
        {
            StartCoroutine(ProcessDamageQueue());
        }
    }
    private IEnumerator ProcessDamageQueue()
    {
        isProcessingQueue = true;

        while (damageQueue.Count > 0)
        {
            DamageQueueData data = damageQueue.Dequeue();
            SpawnDamageText(data.content, data.color);

            // Chờ 0.35 giây trước khi cho phép text tiếp theo xuất hiện
            yield return new WaitForSeconds(0.1f);
        }

        isProcessingQueue = false;
    }

    private void SpawnDamageText(string content, Color color)
    {
        var textDame = CombatManager.GetTextDame();
        textDame.transform.SetParent(CombatManager.parentUIFight);

        // 1. Reset scale về 1 trước khi bắt đầu hiệu ứng (theo yêu cầu của bạn)
        textDame.transform.localScale = Vector3.one * 0.65f;

        textDame.transform.position = this.transform.position;

        // 1. Định nghĩa biên độ ngẫu nhiên cơ bản (khi chỉ có 1-2 text)
        int baseRandomRange = 50;

        // 2. Tính toán biên độ dựa trên activeTextCount
        // Ví dụ: Mỗi text đang hiện hữu sẽ cộng thêm 40 đơn vị vào khoảng random
        int currentRange = baseRandomRange + (activeTextCount * 35);

        // Giới hạn max để tránh text bay quá xa khỏi màn hình (ví dụ max là 350)
        currentRange = Mathf.Min(currentRange, 150);

        // 3. Thực hiện Random trong khoảng đã tính
        int randomX = UnityEngine.Random.Range(-currentRange, currentRange);

        // Áp dụng vào vị trí
        Vector3 startOffset = new Vector3(randomX, 300 + (activeTextCount * 30f), 0);
        textDame.transform.localPosition += startOffset;

        activeTextCount++;

        // Thiết lập nội dung
        textDame.text = content;
        color.a = 1;
        textDame.color = color;

        // 2. TÍNH TOÁN THỜI GIAN (Càng đông càng nhanh)
        // 1.5f là thời gian gốc khi chỉ có 1 text.
        // Mỗi text tiếp theo sẽ giảm thời gian đi 0.1s (hoặc theo tỉ lệ bạn muốn)
        float time = 1.5f - (activeTextCount * 0.1f);

        // Khống chế thời gian tối thiểu (ví dụ không nhanh hơn 0.6s) để người chơi kịp nhìn
        time = Mathf.Max(time, 0.6f);
        float flyHeight = 150f;

        // --- HIỆU ỨNG TỔNG HỢP ---
        DOVirtual.DelayedCall(time / 4, () =>
        {
            activeTextCount--;
            if (activeTextCount < 0) activeTextCount = 0;
        });
            
        // Di chuyển lên trên
        textDame.transform.DOLocalMoveY(textDame.transform.localPosition.y + flyHeight, time/4)
            .SetEase(Ease.OutQuad);

        // 2. Co nhỏ scale về 0
        textDame.transform.DOScale(Vector3.zero, time/2)
            .SetEase(Ease.InBack); // InBack tạo cảm giác thu nhỏ hơi nhanh dần

        // 3. Mờ dần và thu hồi
        textDame.DOFade(0, time).OnComplete(() => {
         
            textDame.gameObject.Recycle();
        });
    }
    public SecureInt khiKim;
    public SecureInt khiMoc;
    public SecureInt khiThuy;
    public SecureInt khiHoa;
    public SecureInt khiTho;
    public SecureInt khiMa;

    public bool IsDebuffTuoiTac => characterStatsBase.tuoiTac.Value > characterStatsBase.thoNguyen.Value;
    public int CurrentIndexDanDoc => GameManager.GetDanDocValue(characterStatsBase.danDoc).index;


    public Dictionary<BaseData, SecureInt> skillTichLuy = new Dictionary<BaseData, SecureInt>();

    public CharacterStatsSecure characterStatsBase = new(); // khi end combat thì characterStatsBase sẽ lấy hp từ hp của stats Temp
    public CharacterStatsSecure characterStatsTemp = new();
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



    public bool IsSkillTichLuyCanUse(BaseData thanThong, SecureInt ValueNeedSave , SecureInt require )
    {
        // 1. Tìm chính xác cái Key đang nằm trong Dictionary dựa trên điều kiện của bạn
        var key = skillTichLuy.Keys.FirstOrDefault(x =>
                    x != null &&
                    x.Id == thanThong.Id 
                  );

        // 2. Nếu không tìm thấy Key phù hợp, trả về false ngay lập tức
        if (key == null)
        {
            Debug.Log("key null");
            key = thanThong;
            skillTichLuy.Add(thanThong, ValueNeedSave);
        }
        else
        {
            skillTichLuy[key] += ValueNeedSave;

        }

        // 3. Kiểm tra số lượng tích lũy
        if (skillTichLuy[key] >= require)
        {
            // Trừ trực tiếp vào giá trị của Dictionary thông qua Key đã tìm thấy
            skillTichLuy[key] -= require;
            if (skillTichLuy[key] <= 0)
            {
                skillTichLuy.Remove(key);
            }    
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
    [Serializable]
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
    public void SetBuffLogic(EBuffAndDeBuff type, int newValue)
    {
        if (buffAndDebuffCount.TryGetValue(type, out var current))
        {
            buffAndDebuffCount[type] = newValue;
        }
        else
        {
            buffAndDebuffCount.Add(type,newValue);
        }
        Observer.Notify(EventList.Combat.ADD_BUFF_UI, this, type, (int)newValue, (int)current);
    }
    [Button]
    public void AddBuffAndDebuffCount(EBuffAndDeBuff type, int count)
    {
        if (count == 0) return;

        buffAndDebuffCount.TryGetValue(type, out var current);

        SecureInt newValue = current + count;
        buffAndDebuffCount[type] = newValue;

        Observer.Notify(EventList.Combat.ADD_BUFF, this, type, (int)newValue, (int)current);
        Observer.Notify(EventList.Combat.ADD_BUFF_UI, this, type, (int)newValue, (int)current);

    }
    [Button]
    public void TryGetCountBuff(EBuffAndDeBuff type)
    {

        SecureInt count = 0;
        buffAndDebuffCount.TryGetValue(type, out count);
        Debug.Log(type + " " + count.Value);


    }
    public void RemoveBuffAndDebuffCount(EBuffAndDeBuff type, int count , Action actionRemoveDone = null)
    {
        if (!buffAndDebuffCount.TryGetValue(type, out SecureInt current))
            return; // không có buff thì khỏi remove
        if (count == 0) return;
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

        Observer.Notify(EventList.Combat.REMOVE_BUFF, this, type, (int)newValue, (int)current);
        Observer.Notify(EventList.Combat.ADD_BUFF_UI, this, type, (int)newValue, (int)current);

    }
    [Button]
    public int GetThanThuc()
    {
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThanThuong, out SecureInt thanThuong);
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NgungThan, out SecureInt ngungThan);
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TinhHonChiLuc, out SecureInt TinhHonChiLuc);

        int thanThuc = characterStatsTemp.thanThuc + TinhHonChiLuc + ngungThan - thanThuong;
        thanThuc = Mathf.Clamp(thanThuc, 0, int.MaxValue);
        Debug.Log("thanThuc" + thanThuc);
        return thanThuc;

    }    
    public int GetToDoBay()
    {
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TangToc, out SecureInt tangToc);
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TroiChan, out SecureInt troiChan);
        return  characterStatsTemp.tocDoBay  + tangToc - troiChan;

    }
    void LoadCongPhapCombatData(SecureInt idCP)
    {
        if (idCP == null)
        {
            Debug.Log("idCP bị null, không thể tiếp tục xử lý!");
            return;
        }

        var CP = Pool.Get<GameManager>().GetCongPhapWithId(idCP.Value);
        if (CP == null)
        {
            Debug.Log($"Không tìm thấy CongPhap trong Data với ID: {idCP.Value}");
            return;
        }

        // 1. Kiểm tra InventoryManager và danh sách sở hữu
        if (Client.Instance?.CharacterData?.InventoryManager?.CongPhapOwners == null)
        {
            Debug.Log("Danh sách CongPhapOwners bị null! Hãy kiểm tra lại dữ liệu Inventory khi Load.");
        }
        else
        {
            var ownerData = Client.Instance.CharacterData.InventoryManager.CongPhapOwners
                            .FirstOrDefault(x => x.Id == idCP.Value);

            if (ownerData == null)
            {
                Debug.Log($"Cảnh báo: ID {idCP.Value} có trong list IdEquip nhưng không tìm thấy trong Inventory.");
            }

            // 2. Kiểm tra danh sách congPhapEquip trước khi Add
            if (congPhapEquip == null)
            {
                Debug.Log("Danh sách 'congPhapEquip' chưa được khởi tạo (null)! Đang tự động khởi tạo mới...");
                congPhapEquip = new List<CongPhapMono>();
            }

            // Tính toán Level an toàn
            int levelRecord = (ownerData != null) ? ownerData.Level : 1;

            congPhapEquip.Add(new CongPhapMono
            {
                congPhap = CP,
                level = levelRecord
            });
        }
    }
    void LoadData()
    {
        foreach (var item in characterStatsBase.thanThongEquip)
        {
            ThanThong thanThong = Pool.Get<GameManager>().GetThanThongWithId(item.Value);
            if(thanThong != null )
            {
                thanThongEquip.Add(thanThong);

            }

        }
        LoadCongPhapCombatData(characterStatsBase.chuTu1Used);
        LoadCongPhapCombatData(characterStatsBase.chuTu2Used);
        LoadCongPhapCombatData(characterStatsBase.phuTu1Used);
        LoadCongPhapCombatData(characterStatsBase.phuTu2Used);
        LoadCongPhapCombatData(characterStatsBase.phuTu3Used);
        LoadCongPhapCombatData(characterStatsBase.phuTu4Used);
        LoadCongPhapCombatData(characterStatsBase.donThuatUsed);
        foreach (var item in characterStatsBase.ngoDaoSaves)
        {
            AllMaps.NgoDao.NgoDao ngoDao = Pool.Get<GameManager>().GetNgoDaoWithId(item.Value);
            if (ngoDao != null &&ngoDao is NgoDaoCombat ngoDaoCombat)
            {
                ngoDaoCombats.Add(ngoDaoCombat);

            }

        }
        foreach (var item in characterStatsBase.danDuocCombat)
        {
            var data = Pool.Get<GameManager>().GetDanDuocWithId(item.id);
            if (data != null)
            {
                danDuocCombats.Add(new CountStack<BaseDanDuocCombat>
                {
                    type = data,
                    count = item.value // Sửa lại đúng tên biến ở đây
                });
            }

        }
    }
    public void Init(GameManager.CharCombatInit charCombatInit )
    {
        var characterStat = charCombatInit.charStats;
        characterVisualControl.Init(charCombatInit.eTypeVisual, characterStat);
        characterStatsBase = characterStat;
        characterStatsTemp = characterStat.Clone();
        LoadData();
        

        thanThongCoolDown.Clear();
        currentCharGender.gameObject.SetActive(true);
        characterVisualControl.AnimIdle();
        thanThongUseThisComBat.Clear();
        InitTotalKhi();

        var getDanDoc = GameManager.GetDanDocValue(characterStatsBase.danDoc.Value);

        if (CurrentIndexDanDoc == 2)
        {
            totalKhiRandom -= 1;
        }
        else if (CurrentIndexDanDoc == 3)
        {
            totalKhiRandom -= 2;

        }
        else if (CurrentIndexDanDoc == 4)
        {
            totalKhiRandom -= 2;

        }

        if(IsDebuffTuoiTac)
        {
            totalKhiRandom -= 2;

        }

        congPhapDynamic.Clear();
        skillTichLuy.Clear();
        foreach (var cp in congPhapEquip)
        {
            AddCPDynamic(cp);
        }
        foreach (var item in GetStaticCombatEffects())
        {
            item.InitPosCallExecute(this);

        }
        Debug.Log("dcmmm " + congPhapDynamic.Count);
      

        if (currentVuKhiVisual != null)
        {
            currentVuKhiVisual.Init(characterStatsBase.vuKhi.Item.IconFight);
        }

        //Test
        


    }
    public List<IStaticCombatEffect> GetStaticCombatEffects()
    {
        // Gom tất cả các danh sách lẻ vào một danh sách interface chung
        List<IStaticCombatEffect> effects = new List<IStaticCombatEffect>();

        // Dùng AddRange để thêm toàn bộ phần tử từ các List con
        // Giả sử congPhapEquip, ngoDaoCombats, danDuocCombats đều là List các object kế thừa IStaticCombatEffect
        if (congPhapEquip != null) effects.AddRange(congPhapEquip.Select(x => x.congPhap));
        if (ngoDaoCombats != null) effects.AddRange(ngoDaoCombats);
        if (danDuocCombats != null) effects.AddRange(danDuocCombats.Select(x=>x.type));

        return effects;
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
        Observer.Notify(EventList.Combat.ADJUST_LINHKHI, this,elinhKhi,count );

    }
    public SecureInt totalKhi;

    public SecureInt totalKhiRandom;

    public SecureInt firstBaseHapThuLinhKhi;

    public int CurrentLinhKhi => khiKim + khiMoc+khiThuy+khiHoa+khiTho+khiMa;

    public int baseCountHapThuLinhKhi => CurrentTurn != 1 ? firstBaseHapThuLinhKhi : totalKhiRandom - CurrentLinhKhi;
    public SecureInt countHapThuLinhKhiNewTurn;

    private void InitTotalKhi()
    {
        int level = characterStatsTemp.level;

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
        firstBaseHapThuLinhKhi = totalKhiRandom -totalKhi;

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
        Observer.Notify(EventList.Combat.ADJUST_LINHKHI, this, eLinhKhi, -count);

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
    public List<LinhKhiTieuTan> GetLinhKhiTieuTanForTotalLinhKhi()
    {
        System.Random rand = new System.Random();
        Dictionary<ELinhKhi, int> dictLinhKhiTemp = new Dictionary<ELinhKhi, int>
        {
            { ELinhKhi.Kim,  khiKim },
            { ELinhKhi.Moc,  khiMoc },
            { ELinhKhi.Thuy, khiThuy },
            { ELinhKhi.Hoa,  khiHoa },
            { ELinhKhi.Tho,  khiTho },
            { ELinhKhi.Ma,   khiMa }
        };

        List<LinhKhiTieuTan> linhKhiTieuTan = new();

        while (true)
        {
            int currentLinhKhi = dictLinhKhiTemp.Sum(x => x.Value);

            if (currentLinhKhi <= totalKhi)
            {
                break;
            }
            else
            {
                // 1. Lấy danh sách các hệ hiện đang có số lượng > 0
                var availableKeys = dictLinhKhiTemp.Where(x => x.Value > 0).Select(x => x.Key).ToList();

                if (availableKeys.Count > 0)
                {
                    // 2. Chọn ngẫu nhiên 1 hệ trong danh sách có sẵn
                    ELinhKhi randomKey = availableKeys[rand.Next(availableKeys.Count)];

                    // 3. Trừ 1 đơn vị trong Dictionary
                    dictLinhKhiTemp[randomKey] -= 1;

                    // 4. Lưu vào danh sách tiêu tán (Bạn có thể gộp các Count giống nhau sau hoặc add mới)
                    linhKhiTieuTan.Add(new LinhKhiTieuTan { eLinhKhi = randomKey, count = 1 });
                }
                else
                {
                    // Trường hợp kho đã sạch bách linh khí nhưng vẫn chưa thỏa điều kiện totalKhi
                    break;
                }
            }
        }
        return linhKhiTieuTan;



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
        if(buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NhieuHon, out SecureInt NhieuHon))
        {
            totalKhiHapThu /= 2;
        }
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
        float pKim = characterStatsTemp.percentKim;
        float pMoc = characterStatsTemp.percentMoc;
        float pThuy = characterStatsTemp.percentThuy;
        float pHoa = characterStatsTemp.percentHoa;
        float pTho = characterStatsTemp.percentTho;
        float pMa = characterStatsTemp.percentMa;

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
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TichDoc, out SecureInt TichDoc))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.TrungDoc, TichDoc);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SucThe, out SecureInt sucThe))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.SucThe, sucThe);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.AnhNac, out SecureInt anhNac))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.AnhNac, anhNac);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DeTonThuong, out SecureInt deTonThuong))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.DeTonThuong, deTonThuong);
        }
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChanLongChiHuyet,out SecureInt chanLongChiHuyet);
        if(chanLongChiHuyet > 0)
        {
            for (int i = 0; i < 10; i++)
            {
                var debuffKeys = buffAndDebuffCount.Keys
                .Where(key => BaseSkill.eDebuffs.Contains(key))
                .ToList();
                // 2. Kiểm tra xem có debuff nào để xóa không
                if (debuffKeys.Count > 0)
                {
                    // 3. Chọn ngẫu nhiên một key trong danh sách debuff đã lọc
                    var randomDebuffKey = debuffKeys[CombatStatic.IndexRandom(0, debuffKeys.Count)];

                    // 4. Thực hiện xóa 1 Count
                    RemoveBuffAndDebuffCount(randomDebuffKey, 1);

                }
                else
                {
                    break;
                }    
              
            }
           

        }
       
        RemoveBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, 1);
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.GiamTonThuong, out SecureInt giamTonThuong))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.GiamTonThuong, giamTonThuong);
        }

        thanThongUseThisComBat.Add(new List<PlaySkillData>());
        if(isCooldownWeapon)
        {
            isCooldownWeapon = false;
        }    

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
        if (characterStatsBase.ability.HasFlag(CreateCharacter.CharStats.Ability.LongTocHuyetMach))
        {
            Healing((int)(0.05f * characterStatsTemp.hpMax));
        }
        RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChayMau, 1);

       
        
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TriLieu, out SecureInt triLieu))
        {
            Healing(triLieu);
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.TriLieu, 1);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrienNhieu, out SecureInt trienNhieu))
        {

            

            RemoveBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu, 1);

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
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NguKiemHiep, out SecureInt nguKiemHiep))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.NguKiemHiep, nguKiemHiep);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NguKhiHiep, out SecureInt nguKhiHiep))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.NguKhiHiep, nguKhiHiep);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NguKimHiep, out SecureInt nguKimHiep))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.NguKimHiep, nguKimHiep);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NguMocHiep, out SecureInt nguMocHiep))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.NguMocHiep, nguMocHiep);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NguThuyHiep, out SecureInt nguThuyHiep))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.NguThuyHiep, nguThuyHiep);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NguHoaHiep, out SecureInt NguHoaHiep))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.NguHoaHiep, NguHoaHiep);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NguThoHiep, out SecureInt NguThoHiep))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.NguThoHiep, NguThoHiep);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NguThanThucHiep, out SecureInt NguThanThucHiep))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.NguThanThucHiep, NguThanThucHiep);
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
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TinhHonChiLuc, out SecureInt TinhHonChiLuc))
        {

            RemoveBuffAndDebuffCount(EBuffAndDeBuff.TinhHonChiLuc, TinhHonChiLuc);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NguKim, out SecureInt NguKim))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.NguKim, NguKim);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NguMoc, out SecureInt NguMoc))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.NguMoc, NguMoc);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NguThuy, out SecureInt NguThuy))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.NguThuy, NguThuy);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NguHoa, out SecureInt NguHoa))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.NguHoa, NguHoa);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NguTho, out SecureInt NguTho))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.NguTho, NguTho);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TichHoa, out SecureInt TichHoa))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, TichHoa);
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.TichHoa, TichHoa);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TichKim, out SecureInt TichKim))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.TichKim, TichKim);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TichMoc, out SecureInt TichMoc))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.TichMoc, TichMoc);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TichThuy, out SecureInt TichThuy))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.TichThuy, TichThuy);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TichDoc, out SecureInt TichDoc))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.TichDoc, TichDoc);
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.PhanTonThuong, out SecureInt PhanTonThuong))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.PhanTonThuong, PhanTonThuong);
        }
        isSkipTieuTanLinhKhi = false;
       
    }

    public int GetDameBuffForTarget()
    {
        int dameBuff = 0;
        foreach (var item in allDamages)
        {
            if (item > 0)
            {
                dameBuff = item;
                break;
            }
        }
        return dameBuff;

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
                    if (allDamages[i] < 0)
                    {
                        allDamages[i] = 0;
                    }    
                }
            }   
            else
            {
                if (sum<0)
                {
                    return;
                }
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
    public void CaculateDameByThanThong(CharacterCombat sender, List<int> dames, BaseSkill.ETypeSkill[] eTypeSkils, bool isCongDon = false)
    {
        
        var listNewDames = CaculateDame(sender, dames, isCongDon, eTypeSkils);
       

        if (sender.characterStatsBase.ability.HasFlag(CreateCharacter.CharStats.Ability.TuyetAmDocThe))
        {
            Debug.Log("sender có TuyetAmDocThe");
            if (sender.characterStatsTemp.danDoc > 50)
            {

                int dameBuff = (int)(GetDameBuffForTarget() * 0.2f);
                Debug.Log(GetDameBuffForTarget() + " co vao buff dame " + dameBuff);
                AddDamageData(new List<int> { dameBuff });
               
            }
        }


    }
    public void NotifyEventCaculateDameForCongPhapFromThanThong(CharacterCombat sender)
    {
        if(allDamages.All(x=>x <=0))
        {
            return;
        }    
        Observer.Notify(EventList.Combat.CACULATE_DAME_BY_THANTHONG, sender, this, allDamages);
        listDameTaked[listDameTaked.Count - 1] += allDamages.Sum();

    }
    private List<int> CaculateDameForStatic(CharacterCombat sender, List<int> dames, bool isCongDon = false, BaseSkill.ETypeSkill[] eTypeSkils = null)
    {
        if (dames.All(x => x <= 0))
        {
            return dames;
        }
        if (sender != null && eTypeSkils != null && dames.Count > 0)
        {   // Build decorator chain for this damage value
            IDamageModifier damageModifier = new PlainDamageModifier(dames[0]);

            //// Apply sender's offensive buffs
            //damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.ChanKim, eTypeSkils);
            //damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.ChanMoc, eTypeSkils);
            //damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.ChanThuy, eTypeSkils);
            //damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.ChanHoa, eTypeSkils);
            //damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.ChanTho, eTypeSkils);
            //damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.ChanKiem, eTypeSkils);
            //damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.ChanThan, eTypeSkils);
            //damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.ChanKhi, eTypeSkils);
            // Apply sender's offensive buffs
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.NguKimHiep, eTypeSkils);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.NguMocHiep, eTypeSkils);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.NguThuyHiep, eTypeSkils);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.NguHoaHiep, eTypeSkils);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.NguThoHiep, eTypeSkils);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.NguKiemHiep, eTypeSkils);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.NguThanThucHiep, eTypeSkils);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.NguKhiHiep, eTypeSkils);
            // Apply SucLuc and Diem
            //damageModifier = new BuffDameDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.SucLuc);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.Diem, eTypeSkils);

            // Apply defender's damage reductions (DeTonThuong)
            damageModifier = new BuffDameDecorator(damageModifier, dames[0], this, EBuffAndDeBuff.DeTonThuong);

            // Apply defender's debuffs (NguKim, NguMoc, etc.)
            damageModifier = new DefenseDebuffDecorator(damageModifier, dames[0], this, EBuffAndDeBuff.NguKim, eTypeSkils);
            damageModifier = new DefenseDebuffDecorator(damageModifier, dames[0], this, EBuffAndDeBuff.NguMoc, eTypeSkils);
            damageModifier = new DefenseDebuffDecorator(damageModifier, dames[0], this, EBuffAndDeBuff.NguThuy, eTypeSkils);
            damageModifier = new DefenseDebuffDecorator(damageModifier, dames[0], this, EBuffAndDeBuff.NguHoa, eTypeSkils);
            damageModifier = new DefenseDebuffDecorator(damageModifier, dames[0], this, EBuffAndDeBuff.NguTho, eTypeSkils);
            // Apply GiảmTonThuong (sender's damage reduction)
            damageModifier = new GiamTonThuongDecorator(damageModifier, dames[0], this);
            // Apply SuongGia (sender's damage reduction)
            damageModifier = new SuongGiaDecorator(damageModifier, dames[0], sender);
            // Calculate final damage
            int finalDame = damageModifier.Calculate();
            for (int i = 0; i < dames.Count; i++)
            {
                dames[i] = finalDame;
            }

        }
        AddDamageData(dames, isCongDon);
        return dames;
    }
    private List<int> CaculateDame(CharacterCombat sender, List<int> dames, bool isCongDon = false, BaseSkill.ETypeSkill[] eTypeSkils = null)
    {
        if (dames.All(x => x <= 0))
        {
            return dames;
        }
        if (sender != null && eTypeSkils != null && dames.Count>0)
        {   // Build decorator chain for this damage value
            IDamageModifier damageModifier = new PlainDamageModifier(dames[0]);

            // Apply sender's offensive buffs
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.ChanKim, eTypeSkils);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.ChanMoc, eTypeSkils);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.ChanThuy, eTypeSkils);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.ChanHoa, eTypeSkils);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.ChanTho, eTypeSkils);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.ChanKiem, eTypeSkils);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.ChanThan, eTypeSkils);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.ChanKhi, eTypeSkils);
            // Apply sender's offensive buffs
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.NguKimHiep, eTypeSkils);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.NguMocHiep, eTypeSkils);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.NguThuyHiep, eTypeSkils);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.NguHoaHiep, eTypeSkils);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.NguThoHiep, eTypeSkils);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.NguKiemHiep, eTypeSkils);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.NguThanThucHiep, eTypeSkils);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.NguKhiHiep, eTypeSkils);
            // Apply SucLuc and Diem
            damageModifier = new BuffDameDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.SucLuc);
            damageModifier = new OffensiveBuffDecorator(damageModifier, dames[0], sender, EBuffAndDeBuff.Diem, eTypeSkils);

            // Apply defender's damage reductions (DeTonThuong)
            damageModifier = new BuffDameDecorator(damageModifier, dames[0], this, EBuffAndDeBuff.DeTonThuong);

            // Apply defender's debuffs (NguKim, NguMoc, etc.)
            damageModifier = new DefenseDebuffDecorator(damageModifier, dames[0], this, EBuffAndDeBuff.NguKim, eTypeSkils);
            damageModifier = new DefenseDebuffDecorator(damageModifier, dames[0], this, EBuffAndDeBuff.NguMoc, eTypeSkils);
            damageModifier = new DefenseDebuffDecorator(damageModifier, dames[0], this, EBuffAndDeBuff.NguThuy, eTypeSkils);
            damageModifier = new DefenseDebuffDecorator(damageModifier, dames[0], this, EBuffAndDeBuff.NguHoa, eTypeSkils);
            damageModifier = new DefenseDebuffDecorator(damageModifier, dames[0], this, EBuffAndDeBuff.NguTho, eTypeSkils);
            // Apply GiảmTonThuong (sender's damage reduction)
            damageModifier = new GiamTonThuongDecorator(damageModifier, dames[0], this);
            // Apply SuongGia (sender's damage reduction)
            damageModifier = new SuongGiaDecorator(damageModifier, dames[0], sender);
            // Calculate final damage
            int finalDame = damageModifier.Calculate();
            for (int i = 0; i < dames.Count; i++)
            {
                dames[i] = finalDame;
            }
            
        }
        AddDamageData(dames, isCongDon);
        return dames;
    }
    public void TakeDameByStatic(CharacterCombat sender, List<int> dames,  bool isCongDon = false , BaseSkill.ETypeSkill[] eTypeSkils = null , bool isDef = true)
    {
        if(dames.All(x=>x <=0 ))
        {
            return;
        }    
        var newListDames = CaculateDameForStatic(sender, dames, isCongDon, eTypeSkils);
        if (sender != null && eTypeSkils != null)
        {
            Observer.Notify(EventList.Combat.CHARACTER_TAKE_DAME_BY_STATIC, sender, this, newListDames, eTypeSkils);

        }
        TakeDameForStatic(sender,isDef);
    }
    public void TakeDameForStatic(CharacterCombat sender, bool isDef)
    {
        TakeDame(sender, isDef);

    }
    public void TakeDameForThanThong(CharacterCombat sender, bool isDef)
    {
        NotifyEventCaculateDameForCongPhapFromThanThong(sender);
        TakeDame(sender,isDef);
    }
    [Button]
    private void TakeDame(CharacterCombat sender , bool isDef)
    {
        if(sender != null)
        {
            if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoaDiem, out SecureInt hoaDiem))
            {
                sender.TakeDameByStatic(this, new List<int> { hoaDiem }, false, new[] { BaseSkill.ETypeSkill.Hoa });

            }
            if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.PhanTonThuong, out SecureInt phanTonThuong))
            {
                sender.TakeDameByStatic(this, new List<int> { phanTonThuong }, false);

            }
           
        }
        if (buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TichKim, out SecureInt TichKim))
        {
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.DeTonThuong, TichKim);
        }
        
        if (allDamages.Count <= 0) return;
        if (sender != null)
        {
            Observer.Notify(EventList.Combat.CHARACTER_BEFORE_TAKE_DAME, sender, this);
        }
        List<DameData> listDameData = new List<DameData>();
        for (int i = 0; i < allDamages.Count; i++)
        {
            int finalDame = 0;
            int finalDameHoTrao = 0;

            if (!isDef)
            {
                TakeDame(sender, allDamages[i], isDef,false,ref finalDame,ref finalDameHoTrao);
                listDameData.Add(new DameData
                {
                    Dame = finalDame,
                    IsDef = isDef,
                    IsNeTranh = false,
                    DameHoTrao = finalDameHoTrao
                });
                continue;
            }    
            if (allDamages[i] != 0)
            {
                bool isActiveHuyenVu = CombatStatic.IsActiveHuyenVu(null, this);
                bool isActiveAnNac = CombatStatic.IsActiveAnNac(null, this);
                bool isActiveAnhNac = CombatStatic.IsActiveAnhNac(null, this);

                if (isActiveHuyenVu || isActiveAnNac)
                {
                    allDamages[i] = -1;
                }
                if (sender != null)
                {
                    Observer.Notify(EventList.Combat.CHECK_NE_TRANH, sender, this, i);
                }
            }
            var dame = allDamages[i];
            bool isNeTranh = dame == -1;


            TakeDame(sender, allDamages[i], isDef, isNeTranh, ref finalDame, ref finalDameHoTrao);
            listDameData.Add(new DameData
            {
                Dame = finalDame,
                IsDef = isDef,
                IsNeTranh = isNeTranh,
                DameHoTrao = finalDameHoTrao
            });

        }
        Observer.Notify(EventList.Combat.CHARACTER_AFTER_TAKE_DAME_ALL, sender,this, listDameData);
        allDamages.Clear();
    }
   
    private void TakeVisualDameHoTrao(int dameHoTrao)
    {
        // hộ tráo đã được đồng bộ từ event sync buff rồi
        SetTextDameAnim($"-{dameHoTrao}", Color.cyan);

    }
    private void TakeVisualDame(int dame , bool isDef)
    {
        characterStatsTemp.hp -= dame;
        characterStatsTemp.hp = Mathf.Clamp(characterStatsTemp.hp, 0, characterStatsTemp.hpMax);
        StartAnimHit();
        SetTextDameAnim($"-{dame}", isDef ? Color.red : Color.purple);
        Observer.Notify(EventList.Combat.CHARACTER_ADJUST_HP);

    }
    public void TakeDameVisualPvP(DameData dameDataServer)
    {
        if(dameDataServer != null)
        {
            if(dameDataServer.IsNeTranh)
            {
                NeTranh(null);
                return;
            }
            if(dameDataServer.IsDef)
            {
                TakeVisualDame(dameDataServer.Dame,true);
                TakeVisualDameHoTrao(dameDataServer.DameHoTrao);

            }
            else
            {
                TakeVisualDame(dameDataServer.Dame, false);

            }
        }
    }
    void NeTranh(CharacterCombat sender)
    {
        SetTextDameAnim("Né Tránh!", Color.whiteSmoke);
        if (sender != null)
        {
            Observer.Notify(EventList.Combat.NE_TRANH_SUCCESS, sender, this);
        }
    }
    private void TakeDame(CharacterCombat sender ,int dame , bool isDef , bool isNeTranh ,ref int finalDame, ref int finalDameHoTrao)
    {
      
        if(isNeTranh)
        {
            NeTranh(sender);
            return;
        }
        if (isDef)
        {
            
            int hotraoMax = buffAndDebuffCount.GetValueOrDefault(EBuffAndDeBuff.HoTrao, 0);
            hotraoMax = Mathf.Clamp(hotraoMax, 0, int.MaxValue);
            RemoveBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, dame);
            int textDameHoTrao = hotraoMax > dame ? dame : hotraoMax;
            if(hotraoMax > 0 )
            {
                TakeVisualDameHoTrao(textDameHoTrao);
            }    

            dame -= hotraoMax;
        }    
        dame = Mathf.Clamp(dame, 0, int.MaxValue);

        RemoveBuffAndDebuffCount(EBuffAndDeBuff.SucThe, dame);

        Debug.Log(this.gameObject.name + "nhận dame là : " + dame);
        
      

        if (dame > 0)
        {

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
            TakeVisualDame(dame, isDef);

        }

        if (sender != null)
        {
            Observer.Notify(EventList.Combat.CHARACTER_TAKE_DAME, sender, this, dame);
        }
        

    }
    public void SetHp(int hp)
    {
        characterStatsTemp.hp = hp;
        Observer.Notify(EventList.Combat.CHARACTER_ADJUST_HP);

    }

    public virtual bool CheckDie()
    {
        if (characterStatsTemp.hp <= 0)
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

    SecureInt dataHealCurrent = 0;
    public void AddDataHeal(int hp)
    {
        dataHealCurrent.Value += hp;
    }    
    public void Healing(int hp)
    {
        buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.PhaThe, out SecureInt phaThe);

        // Tính toán hệ số giảm hồi phục (mỗi điểm phaThe giảm 50%)
        float reductionFactor = Mathf.Pow(0.5f, phaThe);
        int finalHpGain = Mathf.RoundToInt(hp * reductionFactor);
        AddDataHeal(finalHpGain);
        int currentValueHp = (int)characterStatsTemp.hp + dataHealCurrent.Value;
        Observer.Notify(EventList.Combat.CHARACTER_HEALING, this, currentValueHp, dataHealCurrent.Value);
        Healing();
    }
    public void HealingVisual(int dataHeal)
    {
        // Áp dụng vào chỉ số nhân vật
        characterStatsTemp.hp += dataHeal;


        characterStatsTemp.hp = Mathf.Clamp(characterStatsTemp.hp, 0, characterStatsTemp.hpMax);

        // Hiển thị hiệu ứng số nhảy (Nếu hồi phục > 0 thì mới hiện)
        if (dataHeal > 0)
        {
            SetTextDameAnim($"+{dataHeal}", Color.green);
        }


        var listBuff = transform.GetComponentsInChildren<BuffEffect>();
        foreach (var item in listBuff)
        {
            if (item.eBuff == EBuffAndDeBuff.TriLieu)
            {
                item.ShowEffect();
                break;
            }
        }
        dataHeal = 0;
        Observer.Notify(EventList.Combat.CHARACTER_ADJUST_HP);
    }
    public void Healing()
    {
        HealingVisual(dataHealCurrent.Value);
        dataHealCurrent.Value = 0;
    }

    [Button]
    public void AddKietSuc()
    {
        AddBuffAndDebuffCount(EBuffAndDeBuff.KietSuc, 1);
    }
}
