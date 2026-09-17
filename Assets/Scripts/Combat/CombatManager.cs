using AllMaps.EventReward;
using AllMaps.NgoDao;
using Base;
using CreateCharacter.CharStats;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using LongDH.LongDHModules;
using LongDH.Script.ObjectPool;
using LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using LuyenKhi;
using Network.Models;
using NUnit.Framework;
using Sirenix.OdinInspector;
using Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Combat;
using ExcelDataReader.Log;
using Network;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class LinhKhiTieuTan
{
    public ELinhKhi eLinhKhi;
    public SecureInt count;
}

public enum ETuongQuan
{
    TuongSinh,
    TuongKhac,
    None,
}

public class CombatManager : BaseBehaviour, INetworkEvent.IResultCombat
{
    [System.Serializable]
    public class BuffOrDebuffIcon
    {
        public EBuffAndDeBuff eType;
        public Sprite sprite;
    }

    [SerializeField] private BuffAndDeBuffIconResources iconBuffResources;
    public List<BuffOrDebuffIcon> buffOrDebuffIcons => iconBuffResources.BuffOrDebuffIcons;

    private Dictionary<CharacterCombat.AmKinhEntry, ItemUI> dictAmKinhItemUI =
        new();

    [SerializeField] public CharacterCombat char1;
    [SerializeField] public CharacterCombat char2;
    [SerializeField] private ItemUI prefabBuffIcon;
    public CharacterCombat currentTurn;
    public CharacterCombat targetTurn => currentTurn == char1 ? char2 : char1;
    private float counterTime = 30f;

    [SerializeField] private Text counterTimeText;

    public static readonly Dictionary<ELinhKhi, ELinhKhi> linhKhiLienKich = new Dictionary<ELinhKhi, ELinhKhi>()
    {
        { ELinhKhi.Kim, ELinhKhi.Tho }, // trước Kim là Thổ
        { ELinhKhi.Thuy, ELinhKhi.Kim }, // trước Thủy là Kim
        { ELinhKhi.Moc, ELinhKhi.Thuy }, // trước Mộc là Thủy
        { ELinhKhi.Hoa, ELinhKhi.Moc }, // trước Hỏa là Mộc
        { ELinhKhi.Tho, ELinhKhi.Hoa } // trước Thổ là Hỏa
    };

    public static readonly Dictionary<ELinhKhi, ELinhKhi> TuongSinh = new Dictionary<ELinhKhi, ELinhKhi>()
    {
        { ELinhKhi.Moc, ELinhKhi.Hoa }, // Mộc sinh Hỏa
        { ELinhKhi.Hoa, ELinhKhi.Tho }, // Hỏa sinh Thổ
        { ELinhKhi.Tho, ELinhKhi.Kim }, // Thổ sinh Kim
        { ELinhKhi.Kim, ELinhKhi.Thuy }, // Kim sinh Thủy
        { ELinhKhi.Thuy, ELinhKhi.Moc } // Thủy sinh Mộc
    };

    [SerializeField] private Image progressHpChar1;
    [SerializeField] private Text textHpChar1;
    [SerializeField] private ScrollRect statusChar1;
    [SerializeField] private Image progressHpChar2;
    [SerializeField] private Text textHpChar2;
    [SerializeField] private ScrollRect statusChar2;
    [SerializeField] private Text textNameChar1;
    [SerializeField] private Text textNameChar2;

    [SerializeField] public Text prefabTextDame;
    [SerializeField] private Tooltip buffInfor;
    [SerializeField] public Transform parentUIFight => buffInfor.transform.parent;

    public SecureBool IsDelayGame => CheckDie();
    public SecureBool isDelayEffectSkill;

    public Camera cameraEffectSkillDynamic;
    public Camera cameraEffectSkillLock50;
    public Camera cameraEffectSkillLock5;
    [SerializeField] private Button backCombat;
    [SerializeField] private Image bgCombat;
    [SerializeField] private Text txtTurn;
    public int TurnLinhKhiBaoLoan => 15;

    [Button]
    private void TestShowVisulSkill(BaseSkill baseSkill)
    {
        baseSkill.PlayVisualSkill(currentTurn, targetTurn);
    }

    public void SetUITextTurn()
    {
        // Giả sử turnMgr là bộ quản lý lượt, và turnMgr.CurrentChar là nhân vật đang đi
        string textTurn = $"Hiệp thứ {currentTurn.CurrentTurn + 1} ";

        if (currentTurn == char1)
        {
            textTurn += "<color=#32CD32>(Của bạn)</color>\n";
        }
        else if (currentTurn == char2)
        {
            textTurn += "<color=#FF4500>(Đối thủ)</color>\n";
        }

        // Từ lượt 15 trở đi kích hoạt trạng thái bạo loạn
        if (currentTurn.CurrentTurn >= 15)
        {
            textTurn =
                "<size=25%>(Kết thúc hiệp, tổn hao 10% Khí Huyết tối đa)</size>\n<color=red><b>Linh Khí Bạo Loạn!</b></color>";
        }

        txtTurn.text = textTurn;
    }

    public Text GetTextDame()
    {
        return prefabTextDame.Spawn(this.transform);
    }

    private void SetUIHpBar(float hp, float maxHp, Image progress, Text txt)
    {
        progress.fillAmount = (float)(hp / maxHp);
        txt.text = $"{hp}/{maxHp}";
    }

    private void SetUIHpBar()
    {
        SetUIHpBar(char1.characterStatsTemp.hp, char1.characterStatsTemp.hpMax, progressHpChar1, textHpChar1);
        SetUIHpBar(char2.characterStatsTemp.hp, char2.characterStatsTemp.hpMax, progressHpChar2, textHpChar2);
    }

    private Sprite GetSpriteBuffIcon(EBuffAndDeBuff eBuffAndDeBuff)
    {
        foreach (var item in buffOrDebuffIcons)
        {
            if (item.eType == eBuffAndDeBuff)
            {
                return item.sprite;
            }
        }

        return null;
    }

    private List<int> allThanThongIds => char1.thanThongUseThisComBat
        .SelectMany(innerList => innerList) // San phẳng List<List<PlaySkillData>> thành List<PlaySkillData>
        .Where(data => data?.thanThong != null) // Lọc bỏ các dữ liệu null để tránh lỗi NullReferenceException
        .Select(data => data.thanThong.Id) // Chọn ra id (Giả sử id của ThanThong có kiểu dữ liệu là int/string)
        .ToList();

    public bool CheckDie()
    {
        CharacterCombat characterDie = null;
        CharacterCombat characterWin = null;

        if (targetTurn != null && targetTurn.CheckDie())
        {
            characterDie = targetTurn;
            characterWin = currentTurn;
        }
        else if (currentTurn != null && currentTurn.CheckDie())
        {
            characterDie = currentTurn;
            characterWin = targetTurn;
        }

        if (characterDie != null)
        {
            var gameManager = Pool.Get<GameManager>();

            if (!Pool.Get<EventRewardPanel>().gameObject.activeSelf)
            {
                if (gameManager.currentECombatResult == ETypeResultCombat.None)
                {
                    if (characterWin == char1)
                    {
                        Observer.Notify(EventList.Combat.END_COMBAT, char1, char2, true);
                        EndCombat(ETypeResultCombat.Win);
                    }
                    else
                    {
                        Observer.Notify(EventList.Combat.END_COMBAT, char1, char2, false);
                        EndCombat(ETypeResultCombat.LoseByDied);
                    }
                }

                if (characterWin == char1)
                {
                    EndCombat(ETypeResultCombat.Win);
                }
                else
                {
                    EndCombat(ETypeResultCombat.LoseByDied);
                }
            }

            return true;
        }

        return false;
    }

    public bool IsRealTimeCombat => Pool.Get<GameManager>().currentETypeCombat == ETypeCombat.LuanBanRealTime ||
                                    Pool.Get<GameManager>().currentETypeCombat == ETypeCombat.TongMonChienRealTime;

    public bool IsSync => IsRealTimeCombat && currentTurn == char1 && isSync == true;
    public bool isSync = true;

    private void Awake()
    {
        Pool.Set(this);
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, EndTurn);
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, PlaySkill);
        Observer.AddObserver(EventList.Combat.ADD_SKILL, SetUISkill);
        Observer.AddObserver(EventList.Combat.ADD_BUFF_UI, SetUIBuff);
        Observer.AddObserver(EventList.Combat.UPDATE_AMKINH_THANTHONG, SetUIAmKinh);

        Observer.AddObserver(EventList.Combat.CHARACTER_ADJUST_HP, SetUIHpBar);
        Observer.AddObserver(EventList.Combat.CHARACTER_ADJUST_HP, () => { CheckDie(); });
        Observer.AddObserver(EventList.Combat.RECEIVE_SYNC_PLAYSKILL, ReceiSyncPlaySkill);
        Observer.AddObserver(EventList.Combat.RECEIVE_SYNC_ADJUST_LINHKHI, ReceiSyncAdjustLinhKhi);
        Observer.AddObserver(EventList.Combat.ADJUST_LINHKHI, SendAjustLinhKhi);
        Observer.AddObserver(EventList.Combat.ADD_BUFF, SendSyncBuff);
        Observer.AddObserver(EventList.Combat.REMOVE_BUFF, SendSyncBuff);
        Observer.AddObserver(EventList.Combat.RECEIVE_SYNC_BUFF, ReceiSyncBuff);
        Observer.AddObserver(EventList.Combat.SYNC_START_TURN, StartTurnPVP);
        Observer.AddObserver(EventList.Combat.RECEIVE_SYNC_HEALING, ReceiveSyncHealing);
        Observer.AddObserver(EventList.Combat.CHARACTER_HEALING_AFTER_CALC, SendSyncHealing);
        Observer.AddObserver(EventList.Combat.CHARACTER_AFTER_TAKE_DAME_ALL, SendSyncDame);
        Observer.AddObserver(EventList.Combat.RECEIVE_SYNC_DAME, ReceiveSyncDame);
        Observer.AddObserver(EventList.Combat.RECEIVE_SYNC_USE_PHAPBAO, ReceiveSyncPhapBao);
        Observer.AddObserver(EventList.Combat.RECEIVE_SYNC_ENDTURN,
            () =>
            {
                Observer.Notify(EventList.Combat.END_TURN_COMBAT, currentTurn, targetTurn, new List<LinhKhiTieuTan>());
            });
        Observer.AddObserver(EventList.Combat.RECEIVE_SYNC_CP_DYNAMIC, ReceiveSyncCPDynamic);
        Observer.AddObserver(EventList.Combat.ADD_CP_DYNAMIC, SendSyncCPDynamic);
        Observer.AddObserver(EventList.Combat.RECEIVE_SYNC_USE_EFFECTSKILL, ReceiSyncEffectSkill);
        Observer.AddObserver(EventList.Combat.RECEIVE_SYNC_REMOVE_EFFECTSKILLNODESTROY,
            ReceiSyncEffectRemoveSkillNoDestroy);


        if (char1 == null) Debug.LogError("Char1 bi Null tai Runtime!");
        if (char2 == null) Debug.LogError("Char2 bi Null tai Runtime!");
        var gameConfig = Pool.Get<GameManager>();
        char1.Init(gameConfig.user);
        char2.Init(gameConfig.enemyCombat);
        // test 
        //char1.isAutoCastSkill = true;

        foreach (var item in char1.thanThongEquip)
        {
            item.LoadFirstDataPrefab();
        }

        foreach (var item in char1.congPhapEquip)
        {
            item.congPhap.LoadFirstDataPrefab();
        }

        foreach (var item in char2.thanThongEquip)
        {
            item.LoadFirstDataPrefab();
        }

        foreach (var item in char2.congPhapEquip)
        {
            item.congPhap.LoadFirstDataPrefab();
        }

        if (!IsRealTimeCombat)
        {
            char2.isAutoCastSkill = true;
        }
        else
        {
            char2.isAutoCastSkill = false;
        }

        // new turn sẽ gọi sau toàn bộ event skill call ở endturn là chuẩn
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, NewTurn);
        SetUIHpBar();
        buffInfor.DoHide();
        transform.GetComponentInChildren<PlayerUICombat>().Init();
        textNameChar1.text = char1.characterStatsBase.nameUser;
        textNameChar2.text = char2.characterStatsBase.nameUser;

        if (char1.characterStatsBase.phongCu.Item != null)
        {
            SetUIBuffEquipment(char1, char1.characterStatsBase.phongCu, EquipmentType.Armor);
        }

        if (char2.characterStatsBase.phongCu.Item != null)
        {
            SetUIBuffEquipment(char2, char2.characterStatsBase.phongCu, EquipmentType.Armor);
        }

        if (char1.characterStatsBase.trangSuc.Item != null)
        {
            SetUIBuffEquipment(char1, char1.characterStatsBase.trangSuc, EquipmentType.Jewelry);
        }

        if (char2.characterStatsBase.trangSuc.Item != null)
        {
            SetUIBuffEquipment(char2, char2.characterStatsBase.trangSuc, EquipmentType.Jewelry);
        }

        SetUIBuffDanDuoc(char1);
        SetUIBuffDanDuoc(char2);
        SetUIBuffNgoDao(char1);
        SetUIBuffNgoDao(char2);

        SetEventButton(backCombat,
            () =>
            {
                if (Client.Instance.CharacterData.QuestManager.CurrentMainQuest == 0)
                {
                    return;
                }

                Pool.Get<PopupConfirmAction>().ShowPopConfirmAction("Xác nhận bỏ chạy khỏi trận chiến này?",
                    () => { EndCombat(ETypeResultCombat.LoseByRun); });
            });
        bgCombat.sprite = Pool.Get<GameManager>().spriteBGCombat;
        SoundManager.Instance.PlayMusicCombat();
    }

    public void EndCombat(ETypeResultCombat resultCombatType)
    {
        var gameManager = Pool.Get<GameManager>();
        gameManager.currentECombatResult = resultCombatType;
        if (gameManager.currentETypeCombat == ETypeCombat.LuanBan)
        {
            SuccessResultCombatBase(new(), gameManager.currentECombatResult);
        }
        else if (gameManager.currentETypeCombat == ETypeCombat.LuanBanRealTime ||
                 gameManager.currentETypeCombat == ETypeCombat.TongMonChienRealTime)
        {
            Observer.Notify(EventList.Network.RESULT_COMBAT_REALTIME, this, gameManager.currentETypeCombat,
                char1.characterStatsTemp.hp.Value, char1.characterStatsBase.thanThuc.Value,
                char1.characterStatsBase.thoNguyen.Value, allThanThongIds, char2.characterStatsBase.level.Value,
                gameManager.currentECombatResult); // thằng 1 luoon la user thằng 2 là thằng enemy
        }
        else
        {
            Observer.Notify(EventList.Network.RESULT_COMBAT, this, gameManager.currentETypeCombat,
                char1.characterStatsTemp.hp.Value, char1.characterStatsBase.thanThuc.Value,
                char1.characterStatsBase.thoNguyen.Value, allThanThongIds, char2.characterStatsBase.level.Value,
                gameManager.currentECombatResult); // thằng 1 luoon la user thằng 2 là thằng enemy
        }
    }

    public static bool IsTuongDong(List<LinhKhiTieuTan> linhKhiTieuTan)
    {
        // Lấy danh sách các hệ có Count > 0
        List<ELinhKhi> list = linhKhiTieuTan
            .Where(x => x.count > 0)
            .Select(x => x.eLinhKhi)
            .ToList();

        // Chỉ xét khi có 2 hoặc 3
        if (list.Count == 1)
            return true;

        if (list.Count >= 4)
            return false;

        // Kiểm tra tất cả đều giống nhau
        bool allSame = list.Distinct().Count() == 1;

        return allSame;
    }

    public static ETuongQuan GetETuongQuan(List<LinhKhiTieuTan> linhKhiTieuTan)
    {
        var list = linhKhiTieuTan
            .Where(x => x.count > 0)
            .Select(x => x.eLinhKhi)
            .Distinct()
            .OrderBy(x => (int)x) // Giả định: Kim=0, Thủy=1, Mộc=2, Hỏa=3, Thổ=4
            .ToList();

        if (list.Count <= 1) return ETuongQuan.None;
        if (list.Count >= 4) return ETuongQuan.TuongKhac;

        // Hàm phụ kiểm tra nhanh A có sinh B không (có tính nối vòng)
        bool IsSinh(ELinhKhi a, ELinhKhi b)
        {
            bool sinh =
                (TuongSinh.ContainsKey(a) && TuongSinh[a] == b) ||
                (TuongSinh.ContainsKey(b) && TuongSinh[b] == a);
            return sinh;
        }

        // THẾ TRẬN BỘ 2: [A, B]
        if (list.Count == 2)
        {
            if (IsSinh(list[0], list[1]) || IsSinh(list[1], list[0]))
                return ETuongQuan.TuongSinh;
            return ETuongQuan.TuongKhac;
        }

        // THẾ TRẬN BỘ 3: [A, B, C] (đã sort)
        if (list.Count == 3)
        {
            // Chỉ có 5 bộ 3 duy nhất là Tương Sinh liên hoàn:
            // 1. Kim - Thủy - Mộc (0, 1, 2)
            // 2. Thủy - Mộc - Hỏa (1, 2, 3)
            // 3. Mộc - Hỏa - Thổ (2, 3, 4)
            // 4. Hỏa - Thổ - Kim (3, 4, 0) -> Sort: [Kim, Hỏa, Thổ]
            // 5. Thổ - Kim - Thủy (4, 0, 1) -> Sort: [Kim, Thủy, Thổ]

            bool case123 = IsSinh(list[0], list[1]) && IsSinh(list[1], list[2]);
            bool case4 = IsSinh(list[1], list[2]) && IsSinh(list[2], list[0]); // Hỏa-Thổ, Thổ-Kim
            bool case5 = IsSinh(list[2], list[0]) && IsSinh(list[0], list[1]); // Thổ-Kim, Kim-Thủy

            if (case123 || case4 || case5)
                return ETuongQuan.TuongSinh;
        }

        return ETuongQuan.TuongKhac;
    }

    public static bool IsLienKich(ThanThong thanThong, ThanThong thanThongOldUse)
    {
        if (thanThong == null || thanThongOldUse == null) return false;
        var currentTypes = thanThong.LinhKhiTypes;
        var oldTypes = thanThongOldUse.LinhKhiTypes;
        bool isLienKich = false;
        foreach (var cur in currentTypes)
        {
            // hệ bắt buộc theo vòng
            ELinhKhi mustBeOld = linhKhiLienKich[cur];

            // nếu skill cũ có hệ này → đúng liên kích
            if (oldTypes.Contains(mustBeOld))
            {
                isLienKich = true;
                break;
            }
        }

        return isLienKich;
    }

    [Button]
    public void PlaySkilButton(ThanThong thanThong)
    {
        Observer.Notify(EventList.Combat.PLAY_SKILL, currentTurn, targetTurn, thanThong, new List<LinhKhiTieuTan>());
    }

    private void ResetCounterTime()
    {
        counterTime = 30f;
        counterTimeText.gameObject.SetActive(false);
        var tf = counterTimeText.transform;
        tf.DOKill();
        tf.localScale = Vector2.one * 0.8f;
        tf.DOScale(Vector2.one, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void PlaySkill(params object[] data)
    {
        if (IsDelayGame) return;
        ResetCounterTime();
        CharacterCombat caster = data[0] as CharacterCombat;
        CharacterCombat target = data[1] as CharacterCombat;

        ThanThong thanThong = data[2] as ThanThong;
        List<LinhKhiTieuTan> linhKhiTieuTan = data[3] as List<LinhKhiTieuTan>;
        if (linhKhiTieuTan != null)
        {
            foreach (var item in linhKhiTieuTan)
            {
                Debug.Log(item.eLinhKhi + "cmmm" + item.count);
                caster.TieuTanLinhKhi(item.eLinhKhi, item.count);
            }
        }

        if (thanThong != null)
        {
            caster.PlaySkill(thanThong, linhKhiTieuTan);
            CombatStatic.TrienNhieu(target, caster);
            Observer.Notify(EventList.Combat.BEFORE_EXCUTE_SKILL, caster, target);
            if (caster.IsDebuffTuoiTac)
            {
                caster.TakeDameByStatic(null, new List<int> { (int)(0.05f * caster.characterStatsTemp.hpMax) }, false);
            }

            if (caster.CurrentIndexDanDoc == 4)
            {
                caster.TakeDameByStatic(null, new List<int> { 30 }, false);
            }

            if (caster.IsDebuffTuoiTac || caster.CurrentIndexDanDoc == 4)
            {
                if (CombatStatic.IsActiveWithPercent(50))
                {
                    return;
                }
            }

            if (target.characterStatsBase.ability.HasFlag(CreateCharacter.CharStats.Ability.ThienSinhDiDong))
            {
                if (caster.GetThanThuc() < target.GetThanThuc() && CombatStatic.IsActiveWithPercent(10))
                {
                    return;
                }
            }

            if (caster.skillVoHieuHoa.Count > 0)
            {
                target.RemoveAllThanThongImpactAfterPlaySkill(
                    target.specialThanThongImpactAfterPlaySkill.FirstOrDefault(x => x is AmDuongDong));
                caster.skillVoHieuHoa.Clear();
                return;
            }

            if (IsSync)
            {
                if (!(thanThong is KhuTran && linhKhiTieuTan.Count == 0))
                {
                    Observer.Notify(EventList.Combat.SEND_SYNC_PLAYSKILL, new SyncPlaySkill
                    {
                        CharId = caster.characterStatsBase.idChar.Value,
                        SkillId = thanThong.Id,
                    });
                }
            }

            ThanThong thanThongOldUse = caster.thanThongOldUse != null ? caster.thanThongOldUse.thanThong : null;
            thanThong.Execute(caster, target, data);
            if (thanThong is BaseSkill.LienKich thanThongLienKich && thanThongOldUse != null)
            {
                if (IsLienKich(thanThong, thanThongOldUse))
                {
                    Debug.Log("Liên kích!");
                    thanThongLienKich.ExecuteLienKich(caster, target);
                    Observer.Notify(EventList.Combat.LIEN_KICH, caster, target, thanThong, linhKhiTieuTan);
                    if (caster.characterStatsBase.ability.HasFlag(CreateCharacter.CharStats.Ability.NguKhiTrieuNguyen))
                    {
                        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TuKhi, 1);
                    }
                }
            }

            if (thanThong is BaseSkill.TuongSinh thanThongTuongSinh &&
                GetETuongQuan(linhKhiTieuTan) == ETuongQuan.TuongSinh)
            {
                thanThongTuongSinh.ExecuteTuongSinh(caster, target);
            }

            if (thanThong is BaseSkill.TuongKhac thanThongTuongKhac &&
                GetETuongQuan(linhKhiTieuTan) == ETuongQuan.TuongKhac)
            {
                thanThongTuongKhac.ExecuteTuongKhac(caster, target);
            }

            if (thanThong is BaseSkill.TuongDong thanThongTuongDong && IsTuongDong(linhKhiTieuTan))
            {
                thanThongTuongDong.ExecuteTuongDong(caster, target);
            }

            if (thanThong is BaseSkill.ThanThuc thanThongThanThuc && (caster.GetThanThuc() > target.GetThanThuc()))
            {
                thanThongThanThuc.ExecuteThanThuc(caster, target);
            }

            target.TakeDameForThanThong(caster,
                !thanThong.GetEKeywordSkillHave().Contains(BaseSkill.EKeyWord.ChanThucTonThuong));
            if (thanThong.timeDelayTakeDame > 0)
            {
                isDelayEffectSkill = true;
                parentUIFight.gameObject.SetActive(false);
                DOVirtual.DelayedCall(thanThong.timeDelayTakeDame, () =>
                {
                    isDelayEffectSkill = false;
                    parentUIFight.gameObject.SetActive(true);
                });
            }

            Observer.Notify(EventList.Combat.PLAY_SKILL_COMPLETE, caster, target);
        }
    }

    private void TieuTanLinhKhi(List<LinhKhiTieuTan> linhKhiTieuTan)
    {
        foreach (var item in linhKhiTieuTan)
        {
            Debug.Log(item.eLinhKhi + "cmmm" + item.count);
            currentTurn.TieuTanLinhKhi(item.eLinhKhi, item.count);
        }
    }

    [Button]
    public void EndTurnButton()
    {
        Observer.Notify(EventList.Combat.END_TURN_COMBAT, currentTurn, targetTurn, new List<LinhKhiTieuTan>());
    }

    private void SwapTurn()
    {
        // swap turn
        if (currentTurn == char1)
        {
            currentTurn = char2;
        }
        else
        {
            currentTurn = char1;
        }
    }

    public void EndTurn(params object[] data)
    {
        if (data[2] is List<LinhKhiTieuTan> linhKhiTieuTan)
        {
            TieuTanLinhKhi(linhKhiTieuTan);
        }

        var oldCurrentTurn = currentTurn;
        var oldTargetTurn = targetTurn;
        if (IsSync)
        {
            Observer.Notify(EventList.Combat.SEND_SYNC_ENDTURN,
                new SyncEndTurn { CharId = oldCurrentTurn.characterStatsBase.idChar.Value });
        }

        SwapTurn();
        isSync = false;
        CombatStatic.AnhDich(oldCurrentTurn, oldTargetTurn);
        CombatStatic.KiemKhi(oldCurrentTurn, oldTargetTurn);
        CombatStatic.TrungDoc(oldTargetTurn, oldCurrentTurn);
        CombatStatic.ThienKiem(oldCurrentTurn, oldTargetTurn);
        CombatStatic.ChayMau(oldTargetTurn, oldCurrentTurn);


        if (oldCurrentTurn == char1)
        {
            // nếu user 
            LinhKhiBaoLoan(oldCurrentTurn);
        }
        else
        {
            var gameManager = Pool.Get<GameManager>();
            // nếu enemy 
            if (gameManager.currentETypeCombat == ETypeCombat.AllMaps ||
                gameManager.currentETypeCombat == ETypeCombat.LuanBan ||
                gameManager.currentETypeCombat == ETypeCombat.DauTruong)
            {
                LinhKhiBaoLoan(oldCurrentTurn);
            }
        }

        isSync = true;
        oldCurrentTurn.EndTurn();
        foreach (var item in oldCurrentTurn.characterStatsBase.clanInheritanceEffectSecure)
        {
            if (item.Trigger == Network.Models.Clans.ClanInheritanceTrigger.EndTurn)
            {
                if (item.EffectType != null)
                {
                    ConvertEffectToBuffAction(item.EffectType.Value, item.Value)?.Invoke(oldCurrentTurn, oldTargetTurn);
                }
            }
        }

        ExcuteEquipment(oldCurrentTurn, oldTargetTurn, EquipmentType.Jewelry);
    }

    void LinhKhiBaoLoan(CharacterCombat character) // kết thúc lượt giảm 10% hp nếu turn hiện tại >= 15 tăng tốc độ game
    {
        if (character.CurrentTurn >= TurnLinhKhiBaoLoan)
        {
            character.TakeDameByStatic(null, new List<int> { (int)(0.1f * character.characterStatsTemp.hpMax) }, false,
                null, false);
        }
    }

    public (bool isHave, ItemUI buffIcon) SetUIEffectStatic(CharacterCombat characterCombat, bool isCreate,
        Func<ScrollRect, (bool isHave, ItemUI buffIcon)> actionCheckHave, Sprite spriteIcon, string descIcon,
        int currentValue)
    {
        ScrollRect scrollRect = null;
        if (characterCombat == char1)
        {
            scrollRect = statusChar1;
        }
        else
        {
            scrollRect = statusChar2;
        }

        int scaleX = (int)scrollRect.transform.parent.localScale.x;
        int signX = scaleX / Mathf.Abs(scaleX);

        var result = actionCheckHave?.Invoke(scrollRect);
        bool isHave = result.Value.isHave;
        ItemUI buffIcon = result.Value.buffIcon;

        if (isCreate)
        {
            if (isHave)
            {
                buffIcon.Recycle();
            }
        }
        else
        {
            if (!isHave)
            {
                buffIcon = prefabBuffIcon.Spawn(scrollRect.content.transform);
                buffIcon.transform.localScale = new Vector3(1 * signX, 1, 1);
                buffIcon.InitIcon(spriteIcon, null);
                buffIcon.InitAction(null, () =>
                {
                    Debug.Log("cos goi pointer up buff icon show tip");
                    buffInfor.transform.SetParent(buffIcon.transform);
                    buffInfor.SetText(descIcon);


                    var scrollRect = buffIcon.transform.GetComponentInParent<ScrollRect>();
                    buffInfor.transform.SetParent(scrollRect.transform);
                    // Lấy dấu của scale.x từ parent của scrollRect
                    int parentScaleXSign = (int)Mathf.Sign(scrollRect.transform.parent.localScale.x);
                    buffInfor.DoShow(parentScaleXSign, null);
                });
                isHave = true;
            }

            buffIcon.AddCount(currentValue - buffIcon.count);
        }

        return (isHave, buffIcon);
    }


    public void SetUIAmKinh(params object[] data)
    {
        if (data[0] is CharacterCombat characterCombat)
        {
            CharacterCombat.AmKinhEntry entry = (CharacterCombat.AmKinhEntry)data[1];

            int remove = data.Length > 2 ? (int)data[2] : 0;

            string desc =
                $"{BaseSkill.GetColoredText($"[{entry.ThanThong.nameItem}]", "#ffb143")}: {entry.ThanThong.GetDesc(ThanThong.GetDaiCanhGioi(characterCombat.characterStatsTemp.level))}";
            var result = SetUIEffectStatic(characterCombat, remove == -1, (ScrollRect) =>
            {
                var item = dictAmKinhItemUI.Keys.FirstOrDefault(x =>
                    entry.ThanThong.Id == x.ThanThong.Id && entry.RemainingTurns == x.RemainingTurns);
                if (item != null)
                {
                    return (true, dictAmKinhItemUI[item]);
                }

                return (false, null);
            }, entry.ThanThong.spriteItem, desc, entry.RemainingTurns);
            if (remove != -1)
            {
                if (remove != -2)
                {
                    dictAmKinhItemUI.Add(entry, result.buffIcon);
                }
            }
            else
            {
                dictAmKinhItemUI.Remove(entry);
            }
        }
    }

    public void SetUISkill(params object[] data)
    {
        if (data[0] is CharacterCombat characterCombat)
        {
            BaseSkill thanThong = (BaseSkill)data[1];
            int newValue = (int)data[2];
            int daiCanhGioi = (int)data[4];

            string desc =
                $"{BaseSkill.GetColoredText($"[{thanThong.nameItem}]", "#ffb143")}: {thanThong.GetDesc(daiCanhGioi)}";

            var result = SetUIEffectStatic(characterCombat, newValue <= 0, (scrollRect) =>
            {
                for (int i = 0; i < scrollRect.content.transform.childCount; i++)
                {
                    ItemUI itemUI = scrollRect.content.transform.GetChild(i).GetComponent<ItemUI>();
                    if (itemUI.icon.sprite == thanThong.spriteItem) // cos toon tai
                    {
                        return (true, itemUI);
                    }
                }

                return (false, null);
            }, thanThong.spriteItem, desc, newValue);
        }
    }

    private void SetUIBuffEquipment(CharacterCombat characterCombat, EquipmentClient equipment,
        EquipmentType equipmentType)
    {
        if (characterCombat)
        {
            string desc =
                $"{BaseSkill.GetColoredText($"[{equipment.nameEquip}]", "#ffb143")}: {GameManager.GetDescEquipment(equipment.listEffectEquip, equipmentType)}";


            var result = SetUIEffectStatic(characterCombat, false, (scrollRect) => { return (false, null); },
                equipment.Item.Icon, desc, 1);
        }
    }

    private void SetUIBuffDanDuoc(CharacterCombat characterCombat)
    {
        void SetUIBuffDanDuoc(LinhDan linhDan)
        {
            string desc = $"{BaseSkill.GetColoredText($"[{linhDan.Name}]", "#ffb143")}: {linhDan.Description}";

            var result = SetUIEffectStatic(characterCombat, false, (scrollRect) => { return (false, null); },
                linhDan.Icon, desc, characterCombat.danDuocCombats.FirstOrDefault(x => x.type.Id == linhDan.Id).count);
        }

        if (characterCombat)
        {
            foreach (var item in characterCombat.danDuocCombats)
            {
                SetUIBuffDanDuoc(LuyenDanResources.Instance.FindLinhDan(item.type.Id));
            }
        }
    }

    private void SetUIBuffNgoDao(CharacterCombat characterCombat)
    {
        void SetUIBuffNgoDao(NgoDaoCombat ngoDaoCombat)
        {
            string desc =
                $"{BaseSkill.GetColoredText($"[{ngoDaoCombat.nameItem}]", "#ffb143")}: {ngoDaoCombat.GetDes()}";

            if (ngoDaoCombat)
            {
                var result = SetUIEffectStatic(characterCombat, false, (scrollRect) => { return (false, null); },
                    ngoDaoCombat.spriteItem, desc, 1);
            }
        }

        if (characterCombat)
        {
            foreach (var item in characterCombat.ngoDaoCombats)
            {
                SetUIBuffNgoDao(item);
            }
        }
    }

    public void SetUIBuff(params object[] data)
    {
        if (data[0] is CharacterCombat characterCombat)
        {
            EBuffAndDeBuff buffType = (EBuffAndDeBuff)data[1];
            int newValues = (int)data[2];

            var result = SetUIEffectStatic(characterCombat, newValues <= 0, (scrollRect) =>
            {
                for (int i = 0; i < scrollRect.content.transform.childCount; i++)
                {
                    ItemUI itemUI = scrollRect.content.transform.GetChild(i).GetComponent<ItemUI>();
                    if (itemUI.icon.sprite == GetSpriteBuffIcon(buffType)) // cos toon tai
                    {
                        return (true, itemUI);
                    }
                }

                return (false, null);
            }, GetSpriteBuffIcon(buffType), BaseSkill.GetDescBuffOrDeBuff(buffType), newValues);
        }
    }

    private IEnumerator StartCounterTime()
    {
        ResetCounterTime();
        var duration = 1f;
        var waiter = new WaitForSecondsRealtime(duration);
        while (currentTurn == char1)
        {
            yield return waiter;
            counterTime -= duration;
            counterTimeText.text = ((int)counterTime).ToString();
            if (counterTime <= 0)
            {
                counterTimeText.gameObject.SetActive(false);
                Observer.Notify(EventList.Combat.END_TURN_COMBAT, currentTurn, targetTurn,
                    currentTurn.GetLinhKhiTieuTanForTotalLinhKhi());
                yield break;
            }

            if (counterTime <= 10)
            {
                counterTimeText.gameObject.SetActive(true);
            }
        }
    }

    public void NewTurn()
    {
        ResetCounterTime();
        if (IsRealTimeCombat)
        {
            if (currentTurn == char1)
            {
                StartCoroutine(StartCounterTime());
            }
        }

        SetUITextTurn();
        currentTurn.listDameTaked.Add(0);
        targetTurn.listDameTaked.Add(0);
        Dictionary<ELinhKhi, int> dictHapThuKhi = new Dictionary<ELinhKhi, int>();
        if (!currentTurn.IsKietSuc())
        {
            isSync = false;
            CombatStatic.QuyTon(currentTurn, targetTurn);
            CombatStatic.SucThe(currentTurn, targetTurn);
            CombatStatic.ThieuDot(targetTurn, currentTurn);


            currentTurn.NewTurn();
            foreach (var item in currentTurn.characterStatsBase.clanInheritanceEffectSecure)
            {
                if (item.Trigger == Network.Models.Clans.ClanInheritanceTrigger.StartTurn)
                {
                    if (item.EffectType != null)
                    {
                        ConvertEffectToBuffAction(item.EffectType.Value, item.Value)?.Invoke(currentTurn, targetTurn);
                    }
                }
            }

            CombatStatic.AmKinhTTNewTurn(currentTurn, targetTurn);
            isSync = true;
            ExcuteEquipment(currentTurn, targetTurn, EquipmentType.Armor);
            currentTurn.HapThuLinhKhiNewTurn(out dictHapThuKhi);
        }
        else
        {
            foreach (var type in new[] { EBuffAndDeBuff.KietSuc, EBuffAndDeBuff.DongKet })
            {
                if (currentTurn.buffAndDebuffCount.TryGetValue(type, out var count))
                    currentTurn.RemoveBuffAndDebuffCount(type, count);
            }

            if (IsRealTimeCombat)
            {
                if (IsSync)
                {
                    Observer.Notify(EventList.Combat.END_TURN_COMBAT, currentTurn, targetTurn,
                        new List<LinhKhiTieuTan>());
                }
            }
            else
            {
                Observer.Notify(EventList.Combat.END_TURN_COMBAT, currentTurn, targetTurn, new List<LinhKhiTieuTan>());
            }

            return;
        }

        // param 0 là bool isPlayer == char1 
        Observer.Notify(EventList.Combat.NEW_TURN_COMBAT, currentTurn, targetTurn, currentTurn == char1 ? true : false,
            dictHapThuKhi);
        AutoPlaySkill();
    }


    public void AutoPlaySkill()
    {
        if (currentTurn.isAutoCastSkill)
        {
            StartCoroutine(IEAutoCastSkill());
        }
    }

    private CharacterCombat.PlaySkillData ThanThongAutoUse()
    {
        List<ThanThong> thanThongTemp = new List<ThanThong>();
        foreach (var item in currentTurn.thanThongEquip)
        {
            if (item.IsCanUseGlobal(currentTurn, targetTurn))
            {
                thanThongTemp.Add(item);
            }
        }

        if (thanThongTemp.Count <= 0) return null;

        CharacterCombat.PlaySkillData playSkillData = new();

        if (currentTurn.thanThongCurrentUse != null)
        {
            // Tìm item trong list có cùng dữ liệu với thanThongOldUse
            var itemToMove = thanThongTemp.FirstOrDefault(x => x == currentTurn.thanThongCurrentUse.thanThong);

            if (itemToMove != null)
            {
                thanThongTemp.Remove(itemToMove); // Xóa khỏi vị trí hiện tại
                thanThongTemp.Add(itemToMove); // Thêm lại vào cuối list
            }

            // Sắp xếp lại thanThongTemp theo các tiêu chí:
            // 1. Ưu tiên skill chưa từng dùng hoặc dùng ít nhất trong thanThongUseThisComBat
            // 2. Ưu tiên skill có Tier cao hơn nếu số lần dùng bằng nhau
            thanThongTemp = thanThongTemp
                .OrderBy(skill =>
                {
                    // Duyệt qua các list con trong thanThongUseThisComBat, 
                    // đếm xem skill này xuất hiện bao nhiêu lần trong PlaySkillData.thanThong
                    return currentTurn.thanThongUseThisComBat
                        .Sum(listCon => listCon.Count(playData => playData.thanThong == skill));
                })
                .ThenByDescending(skill => skill.eTierSkill) // Sắp xếp giảm dần theo Tier (Tier cao đứng trước)
                .ToList();
            foreach (var item in thanThongTemp)
            {
                if (item is BaseSkill.LienKich && IsLienKich(item, currentTurn.thanThongCurrentUse.thanThong))
                {
                    List<LinhKhiTieuTan> linhKhiTieuTanTemp = item.GetLinhKhiTieuTanIfPossible(currentTurn);
                    if (linhKhiTieuTanTemp != null && linhKhiTieuTanTemp.Count > 0)
                    {
                        return new CharacterCombat.PlaySkillData
                            { thanThong = item, linhKhiTieuTan = linhKhiTieuTanTemp };
                    }
                }
            }
        }

        foreach (var item in thanThongTemp)
        {
            List<LinhKhiTieuTan> linhKhiTieuTanTemp = new();

            if (item is BaseSkill.TuongDong)
            {
                linhKhiTieuTanTemp = item.GetLinhKhiTieuTanTuongDong(currentTurn);
            }
            else if (item is BaseSkill.TuongSinh)
            {
                linhKhiTieuTanTemp = item.GetLinhKhiETuongQuan(currentTurn, ETuongQuan.TuongSinh);
            }
            else if (item is BaseSkill.TuongKhac)
            {
                linhKhiTieuTanTemp = item.GetLinhKhiETuongQuan(currentTurn, ETuongQuan.TuongKhac);
            }

            if (linhKhiTieuTanTemp != null && linhKhiTieuTanTemp.Count <= 0)
            {
                linhKhiTieuTanTemp = item.GetLinhKhiTieuTanIfPossible(currentTurn);
            }

            if (linhKhiTieuTanTemp != null && linhKhiTieuTanTemp.Count > 0)
            {
                return (new CharacterCombat.PlaySkillData { thanThong = item, linhKhiTieuTan = linhKhiTieuTanTemp });
            }
        }

        return null;
    }

    private IEnumerator IEAutoCastSkill()
    {
        while (true)
        {
            if (!currentTurn.isAutoCastSkill)
            {
                yield break;
            }

            yield return new WaitForSeconds(1);
            ExcuteWeapon();
            var playSkillData = ThanThongAutoUse();
            if (currentTurn.isSkipTieuTanLinhKhi)
            {
                Observer.Notify(EventList.Combat.END_TURN_COMBAT, currentTurn, targetTurn, new List<LinhKhiTieuTan>());
                yield break;
            }

            if (playSkillData != null && playSkillData.linhKhiTieuTan != null &&
                playSkillData.thanThong.IsCanUseLinhKhiRequire(currentTurn))
            {
                Observer.Notify(EventList.Combat.PLAY_SKILL, currentTurn, targetTurn, playSkillData.thanThong,
                    playSkillData.linhKhiTieuTan);
            }
            else
            {
                Observer.Notify(EventList.Combat.END_TURN_COMBAT, currentTurn, targetTurn,
                    currentTurn.GetLinhKhiTieuTanForTotalLinhKhi());
                break;
            }
        }
    }

    private void AddBuffLuyenHon(CharacterCombat charCb)
    {
        if (charCb.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Hon, out SecureInt hon))
        {
            charCb.AddBuffAndDebuffCount(EBuffAndDeBuff.Hon, charCb.characterStatsBase.pointHon.Value);
        }
        else
        {
            charCb.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, charCb.characterStatsBase.pointHon.Value * 2);
        }
    }

    private void StartTurn(CharacterCombat charCombat)
    {
        char1.InitStaticCombatEffect();
        char2.InitStaticCombatEffect();


        currentTurn = charCombat;

        StartEquipment(currentTurn);
        StartEquipment(targetTurn);
        NewTurn();
        StartCoroutine(CheckDieCoroutine());
        AddBuffLuyenHon(currentTurn);
        AddBuffLuyenHon(targetTurn);
    }

    private void StartTurnPVP(params object[] data)
    {
        int charId = (int)data[0];

        var charCombat = charId == char1.characterStatsBase.idChar.Value ? char1 : char2;
        StartTurn(charCombat);
    }

    void Start()
    {
        if (!IsRealTimeCombat)
        {
            var charCb = char1.GetToDoBay() >= char2.GetToDoBay() ? char1 : char2;
            StartTurn(charCb);
        }
    }

    private IEnumerator CheckDieCoroutine()
    {
        var wait = new WaitForSeconds(1f);
        var die = false;
        while (!die)
        {
            CheckDie();
            die = Pool.Get<EventRewardPanel>().gameObject.activeSelf;
            //Debug.Log($"Is die: {die}");
            yield return wait;
        }
    }

    public void ExcuteWeapon()
    {
        if (!currentTurn.isCooldownWeapon)
        {
            ExcuteEquipment(currentTurn, targetTurn, EquipmentType.Weapon);
            currentTurn.isCooldownWeapon = true;
            if (currentTurn.currentVuKhiVisual != null)
            {
                currentTurn.currentVuKhiVisual.ActiveAttack();
            }

            if (IsSync)
            {
                Observer.Notify(EventList.Combat.SEND_SYNC_USE_PHAPBAO, new Network.Models.SyncUsePhapBao
                {
                    CharId = currentTurn.characterStatsBase.idChar.Value,
                });
            }
        }
    }

    public virtual void StartEquipment(CharacterCombat caster)
    {
        List<EffectType> listEffectTypeStartCombat = new List<EffectType>
        {
            EffectType.KimTrong,
            EffectType.MocTrong,
            EffectType.ThuyTrong,
            EffectType.HoaTrong,
            EffectType.ThoTrong,
        };
        // 1. Gom tất cả hiệu ứng từ 3 loại trang bị vào 1 danh sách duy nhất
        var allEquipEffects = caster.characterStatsBase.vuKhi.listEffectEquip
            .Concat(caster.characterStatsBase.phongCu.listEffectEquip)
            .Concat(caster.characterStatsBase.trangSuc.listEffectEquip)
            .Where(e => listEffectTypeStartCombat.Contains(e.Type)); // Chỉ lấy các loại trong list "StartCombat"

        // 2. Duyệt qua danh sách đã lọc và cộng chỉ số
        foreach (var effect in allEquipEffects)
        {
            switch (effect.Type)
            {
                case EffectType.KimTrong:
                    caster.characterStatsTemp.percentKim += effect.Value.Value; // Giả sử SecureInt có thuộc tính .Value
                    break;
                case EffectType.MocTrong:
                    caster.characterStatsTemp.percentMoc += effect.Value.Value;
                    break;
                case EffectType.ThuyTrong:
                    caster.characterStatsTemp.percentThuy += effect.Value.Value;
                    break;
                case EffectType.HoaTrong:
                    caster.characterStatsTemp.percentHoa += effect.Value.Value;
                    break;
                case EffectType.ThoTrong:
                    caster.characterStatsTemp.percentTho += effect.Value.Value;
                    break;
            }
        }
    }

    public void ExcuteEquipment(CharacterCombat caster, CharacterCombat target, EquipmentType equipmentType)
    {
        EquipmentClient equipment = null;
        switch (equipmentType)
        {
            case EquipmentType.Weapon:
                equipment = caster.characterStatsBase.vuKhi;
                break;
            case EquipmentType.Armor:
                equipment = caster.characterStatsBase.phongCu;
                break;
            case EquipmentType.Jewelry:
                equipment = caster.characterStatsBase.trangSuc;

                break;
            default:
                break;
        }

        Debug.Log("ExcuteEquipment " + equipment.listEffectEquip);

        foreach (var item in equipment.listEffectEquip)
        {
            Debug.Log("ExcuteEquipment va" + item.Value.Value);
            Debug.Log("ExcuteEquipment type" + item.Type);

            ConvertEffectToBuffAction(item.Type, item.Value.Value)?.Invoke(caster, target);
        }
    }

    public static Action<CharacterCombat, CharacterCombat> ConvertEffectToBuffAction(
        EffectType Type, int Value)
    {
        switch (Type)
        {
            case EffectType.Bui:

                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Tran, Value); };
            case EffectType.Cat:
                return (caster, target) => { target.AddBuffAndDebuffCount(EBuffAndDeBuff.Cat, Value); };
            case EffectType.Hon:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Hon, Value); };
            case EffectType.ChanHoa:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanHoa, Value); };
            case EffectType.ChanKhi:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanKhi, Value); };
            case EffectType.ChanKiem:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanKiem, Value); };
            case EffectType.ChanKim:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanKim, Value); };
            case EffectType.ChanMoc:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanMoc, Value); };
            case EffectType.ChanThan:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanThan, Value); };
            case EffectType.ChanTho:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanTho, Value); };
            case EffectType.ChanThuy:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanThuy, Value); };
            case EffectType.ChayMau:
                return (caster, target) => { target.AddBuffAndDebuffCount(EBuffAndDeBuff.ChayMau, Value); };
            case EffectType.DeTonThuong:
                return (caster, target) => { target.AddBuffAndDebuffCount(EBuffAndDeBuff.DeTonThuong, Value); };
            case EffectType.Diem:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Diem, Value); };
            case EffectType.GiamTonThuong:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.GiamTonThuong, Value); };
            case EffectType.HapThuHoaLinhKhi:
                return (caster, target) => { caster.HapThuLinhKhi(ELinhKhi.Hoa, Value); };
            case EffectType.HapThuKimLinhKhi:
                return (caster, target) => { caster.HapThuLinhKhi(ELinhKhi.Kim, Value); };
            case EffectType.HapThuLinhKhi:
                return (caster, target) => { caster.HapThuLinhKhi(Value); };
            case EffectType.HapThuMocLinhKhi:
                return (caster, target) => { caster.HapThuLinhKhi(ELinhKhi.Moc, Value); };
            case EffectType.HapThuThoLinhKhi:
                return (caster, target) => { caster.HapThuLinhKhi(ELinhKhi.Tho, Value); };
            case EffectType.HapThuThuyLinhKhi:
                return (caster, target) => { caster.HapThuLinhKhi(ELinhKhi.Thuy, Value); };
            case EffectType.HoaDiem:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoaDiem, Value); };
            case EffectType.HoaTrong:
                break;
            case EffectType.HoTrao:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, Value); };
            case EffectType.KhoiPhucGiaTriSinhMenh:
                return (caster, target) => { caster.Healing(Value); };
            case EffectType.KhoiPhucGiaTriSinhMenhDauLuot:
                return (caster, target) => { caster.Healing(Value); };
            case EffectType.KiemKhi:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, Value); };
            case EffectType.KimTrong:
                break;
            case EffectType.MocTrong:
                break;
            case EffectType.NguHoa:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguHoa, Value); };
            case EffectType.NguHoaHiep:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguHoaHiep, Value); };
            case EffectType.NguKhiHiep:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguKhiHiep, Value); };
            case EffectType.NguKiemHiep:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguKiemHiep, Value); };
            case EffectType.NguKim:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguKim, Value); };
            case EffectType.NguKimHiep:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguKimHiep, Value); };
            case EffectType.NguMoc:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguMoc, Value); };
            case EffectType.NguMocHiep:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguMocHiep, Value); };
            case EffectType.NgungThan:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NgungThan, Value); };
            case EffectType.NguThanHiep:
                return (caster, target) =>
                {
                    Debug.Log("ExcuteEquipment effect" + Value);
                    Debug.Log("ExcuteEquipment effect" + Type);
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguThanThucHiep, Value);
                };
            case EffectType.NguTho:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguTho, Value); };
            case EffectType.NguThoHiep:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguThoHiep, Value); };
            case EffectType.NguThuy:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguThuy, Value); };
            case EffectType.NguThuyHiep:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguThuyHiep, Value); };
            case EffectType.Nhanh:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TangToc, Value); };
            case EffectType.PhanTonThuong:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.PhanTonThuong, Value); };
            case EffectType.PhaThe:
                return (caster, target) => { target.AddBuffAndDebuffCount(EBuffAndDeBuff.PhaThe, Value); };
            case EffectType.QuanQuanh:
                return (caster, target) => { target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu, Value); };
            case EffectType.SucThe:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.SucThe, Value); };
            case EffectType.SuongGia:
                return (caster, target) => { target.AddBuffAndDebuffCount(EBuffAndDeBuff.SuongGia, Value); };
            case EffectType.TaoThanhTonThuong:
                return (caster, target) => { target.TakeDameByStatic(caster, new List<int> { Value }, false); };
            case EffectType.TaoThanhXLanXDiemThuongTon:
                return (caster, target) =>
                {
                    for (int i = 0; i < Value; i++)
                    {
                        target.TakeDameByStatic(caster, new List<int> { Value }, false);
                    }
                };
            case EffectType.ThanThuong:

                return (caster, target) => { target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThanThuong, Value); };
            case EffectType.ThieuDot:
                return (caster, target) => { target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, Value); };
            case EffectType.ThoTrong:
                break;
            case EffectType.ThuyTrong:
                break;
            case EffectType.TichDoc:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TichDoc, Value); };
            case EffectType.TichHoa:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TichHoa, Value); };
            case EffectType.TichKim:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TichKim, Value); };
            case EffectType.TichMoc:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TichMoc, Value); };
            case EffectType.TichThuy:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TichThuy, Value); };
            case EffectType.TriLieuLieu:
                return (caster, target) => { caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TriLieu, Value); };
            case EffectType.TroiDu:
                return (caster, target) => { target.AddBuffAndDebuffCount(EBuffAndDeBuff.TroiChan, Value); };
            case EffectType.TrungDoc:
                return (caster, target) => { target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrungDoc, Value); };
            default:
                break;
        }

        return null;
    }
    // Update is called once per frame

    private void OnDestroy()
    {
        EventList.Combat.RemoveAllCombatObservers();
    }

    private void OnAdsCombatButtonClick()
    {
        Observer.Notify(EventList.Network.WATCH_ADS_COMBAT, Pool.Get<EventRewardPanel>());
    }

    public void SuccessResultCombatBase(RewardSnapshot rewardSnapshot, ETypeResultCombat resultCombaType)
    {
        if (resultCombaType == ETypeResultCombat.Win)
        {
            Pool.Get<EventRewardPanel>().InitShowCombatReward(rewardSnapshot, "Thắng làm vua thua làm giặc!",
                ETypeResultCombat.Win, OnAdsCombatButtonClick);
        }
        else if (resultCombaType == ETypeResultCombat.LoseByDied)
        {
            Pool.Get<EventRewardPanel>()
                .InitShowCombatReward(new(), "Chiến đấu tới hơi thở cuối cùng!", ETypeResultCombat.LoseByDied);
        }
        else if (resultCombaType == ETypeResultCombat.LoseByRun)
        {
            Pool.Get<EventRewardPanel>().InitShowCombatReward(new(), "Ngươi thiêu đốt tinh huyết để bỏ chạy!",
                ETypeResultCombat.LoseByRun);
        }

        EventList.Combat.RemoveAllCombatObservers();
    }

    public void FailResultCombatBase(string message)
    {
        Debug.Log("chỉ có thể là chó hack" + message);
    }

    public void ReceiSyncPlaySkill(params object[] data)
    {
        Debug.Log("ReceiSyncPlaySkill");
        SyncPlaySkill syncPlaySkill = data[0] as SyncPlaySkill;
        var charCombat = syncPlaySkill.CharId == char1.characterStatsBase.idChar.Value ? char1 : char2;
        var charTarget = charCombat.characterStatsBase.idChar.Value == char1.characterStatsBase.idChar.Value
            ? char2
            : char1;
        var thanThong = Pool.Get<GameManager>().GetThanThongWithId(syncPlaySkill.SkillId);
        Observer.Notify(EventList.Combat.PLAY_SKILL, charCombat, charTarget,
            thanThong, new List<LinhKhiTieuTan>());
        charTarget.allDamages.Clear();
    }

    public void ReceiSyncEffectSkill(params object[] data)
    {
        SyncEffectSkill SyncEffectSkill = data[0] as SyncEffectSkill;
        var charCombat = SyncEffectSkill.CharId == char1.characterStatsBase.idChar.Value ? char1 : char2;
        var charTarget = charCombat.characterStatsBase.idChar.Value == char1.characterStatsBase.idChar.Value
            ? char2
            : char1;

        BaseSkill baseSkill = null;
        if (SyncEffectSkill.IsThanThong)
        {
            baseSkill = Pool.Get<GameManager>().GetThanThongWithId(SyncEffectSkill.IdSkill);
            if (baseSkill is not TranPhapTT)
            {
                return;
            }
        }
        else
        {
            baseSkill = Pool.Get<GameManager>().GetCongPhapWithId(SyncEffectSkill.IdSkill);
        }

        var oldSync = isSync;
        try
        {
            isSync = false;
            baseSkill.PlayVisualSkill(charCombat, charTarget);
        }
        finally
        {
            isSync = oldSync;
        }
        //charTarget.allDamages.Clear();
    }

    public void ReceiSyncEffectRemoveSkillNoDestroy(params object[] data)
    {
        SyncEffectSkill SyncEffectSkill = data[0] as SyncEffectSkill;
        var charCombat = SyncEffectSkill.CharId == char1.characterStatsBase.idChar.Value ? char1 : char2;


        BaseSkill baseSkill = null;
        if (SyncEffectSkill.IsThanThong)
        {
            // baseSkill = Pool.Get<GameManager>().GetThanThongWithId(SyncEffectSkill.IdSkill);
            return;
        }
        else
        {
            baseSkill = Pool.Get<GameManager>().GetCongPhapWithId(SyncEffectSkill.IdSkill);
        }

        var oldSync = isSync;
        try
        {
            isSync = false;
            charCombat.RemoveEffectSkillNoDestroy(baseSkill);
        }
        finally
        {
            isSync = oldSync;
        }
        //charTarget.allDamages.Clear();
    }

    public void SendAjustLinhKhi(params object[] data)
    {
        CharacterCombat charCombat = data[0] as CharacterCombat;

        if (IsSync)
        {
            Observer.Notify(EventList.Combat.SEND_SYNC_ADJUST_LINHKHI, new SyncAdjustLinhKhi
            {
                CharId = charCombat.characterStatsBase.idChar.Value,
                Kim = charCombat.khiKim,
                Moc = charCombat.khiMoc,
                Thuy = charCombat.khiThuy,
                Hoa = charCombat.khiHoa,
                Tho = charCombat.khiTho,
                Ma = charCombat.khiMa,
            });
        }
    }

    public void ReceiSyncAdjustLinhKhi(params object[] data)
    {
        SyncAdjustLinhKhi syncAdjustLinhKhi = data[0] as SyncAdjustLinhKhi;
        var charCombat = syncAdjustLinhKhi.CharId == char1.characterStatsBase.idChar.Value ? char1 : char2;
        charCombat.khiKim = syncAdjustLinhKhi.Kim;
        charCombat.khiMoc = syncAdjustLinhKhi.Moc;
        charCombat.khiThuy = syncAdjustLinhKhi.Thuy;
        charCombat.khiHoa = syncAdjustLinhKhi.Hoa;
        charCombat.khiTho = syncAdjustLinhKhi.Tho;
        charCombat.khiMa = syncAdjustLinhKhi.Ma;
    }

    public void SendSyncBuff(params object[] data)
    {
        CharacterCombat charCombat = data[0] as CharacterCombat;
        EBuffAndDeBuff buffType = (EBuffAndDeBuff)data[1];
        int newValue = (int)data[2];

        if (IsSync)
        {
            Observer.Notify(EventList.Combat.SEND_SYNC_BUFF, new SyncBuff
            {
                CharId = charCombat.characterStatsBase.idChar.Value,
                IdBuff = (int)buffType,
                NewBuff = newValue,
            });
        }
    }

    public void ReceiSyncBuff(params object[] data)
    {
        SyncBuff SyncBuff = data[0] as SyncBuff;
        var charCombat = SyncBuff.CharId == char1.characterStatsBase.idChar.Value ? char1 : char2;
        charCombat.SetBuffLogic((EBuffAndDeBuff)SyncBuff.IdBuff, SyncBuff.NewBuff);
    }

    public void SendSyncHealing(params object[] data)
    {
        CharacterCombat charCombat = data[0] as CharacterCombat;
        int healValue = (int)data[2];
        if (IsSync)
        {
            Observer.Notify(EventList.Combat.SEND_SYNC_HEALING, new SyncHealing
            {
                CharId = charCombat.characterStatsBase.idChar.Value,
                Heal = healValue,
            });
            Debug.Log($"[SendSyncHealing] {charCombat.characterStatsBase.nameUser} heal {healValue} hp");
        }
    }

    public void ReceiveSyncHealing(params object[] data)
    {
        SyncHealing syncHealing = data[0] as SyncHealing;
        var charCombat = syncHealing.CharId == char1.characterStatsBase.idChar.Value ? char1 : char2;
        Debug.Log($"[ReceiveSyncHealing] {charCombat.characterStatsBase.nameUser} heal {syncHealing.Heal} hp");
        charCombat.HealingVisual(syncHealing.Heal);
    }

    public void SendSyncDame(params object[] data)
    {
        CharacterCombat charCombatSend = data[0] as CharacterCombat;
        CharacterCombat charCombatTake = data[1] as CharacterCombat;
        List<DameData> listDameData = data[2] as List<DameData>;

        //int healValue = data[2] as int;


        if (IsSync)
        {
            Debug.Log("SendSyncDame" + listDameData.Count);
            int idCharSend = -1;
            if (charCombatSend != null) // char combat send cos the bi null
            {
                idCharSend = charCombatSend.characterStatsBase.idChar.Value;
            }

            Observer.Notify(EventList.Combat.SEND_SYNC_DAME, new SyncDame
            {
                CharSendDame = idCharSend,
                CharTakeDame = charCombatTake.characterStatsBase.idChar.Value,
                Datas = listDameData,
            });
            foreach (var item in listDameData)
            {
                // charCombatTake.TakeDameVisualPvP(item);
                Debug.Log("listDameData dame item" + item.Dame + "dame ho trao " + item.DameHoTrao);
            }
        }
    }

    public void ReceiveSyncDame(params object[] data)
    {
        SyncDame SyncDame = data[0] as SyncDame;
        var charCombatSendDame = SyncDame.CharSendDame == char1.characterStatsBase.idChar.Value ? char1 : char2;
        var charCombatTakeDame = SyncDame.CharTakeDame == char1.characterStatsBase.idChar.Value ? char1 : char2;
        Debug.Log($"Is null: {SyncDame.Datas == null}");
        foreach (var item in SyncDame.Datas)
        {
            charCombatTakeDame.TakeDameVisualPvP(item);
        }
    }

    public void ReceiveSyncPhapBao(params object[] data)
    {
        SyncUsePhapBao SyncUsePhapBao = data[0] as SyncUsePhapBao;
        var charCombat = SyncUsePhapBao.CharId == char1.characterStatsBase.idChar.Value ? char1 : char2;

        charCombat.currentVuKhiVisual.ActiveAttack();
    }

    public void SendSyncCPDynamic(params object[] data)
    {
        CharacterCombat charCombatSend = data[0] as CharacterCombat;
        CongPhap cp = data[1] as CongPhap;
        int newValue = (int)data[2];
        //int healValue = data[2] as int;


        if (IsSync)
        {
            Observer.Notify(EventList.Combat.SEND_SYNC_CP_DYNAMIC, new SyncCPDynamic
            {
                CharId = charCombatSend.characterStatsBase.idChar.Value,
                IdCpDynamic = cp.Id,
                NewCpDynamic = newValue,
            });
        }
    }

    public void ReceiveSyncCPDynamic(params object[] data)
    {
        SyncCPDynamic SyncCPDynamic = data[0] as SyncCPDynamic;
        var charCB = SyncCPDynamic.CharId == char1.characterStatsBase.idChar.Value ? char1 : char2;
        var congPhap = Pool.Get<GameManager>().GetCongPhapWithId(SyncCPDynamic.IdCpDynamic);
        var cpMono = charCB.congPhapEquip.FirstOrDefault(x => x.congPhap.Id == SyncCPDynamic.IdCpDynamic);
        var oldSync = isSync;
        try
        {
            isSync = false;
            if (SyncCPDynamic.NewCpDynamic <= 0)
            {
                charCB.RemoveCPDynamic(congPhap);
            }
            else if (cpMono != null)
            {
                charCB.SetCPDynamic(cpMono, SyncCPDynamic.NewCpDynamic);
            }
        }
        finally
        {
            isSync = oldSync;
        }
    }
}