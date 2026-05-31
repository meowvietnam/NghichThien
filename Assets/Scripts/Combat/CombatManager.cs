using AllMaps.NgoDao;
using Base;
using CreateCharacter.CharStats;
using DG.Tweening;
using LongDH.LongDHModules;
using LongDH.Script.ObjectPool;
using LuyenDan;
using LuyenDan.Inventory;
using LuyenKhi;
using NUnit.Framework;
using Sirenix.OdinInspector;
using Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LongDHModules;
using UnityEngine;
using UnityEngine.UI;
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

public class CombatManager : BaseBehaviour
{
    [System.Serializable]   
    public class BuffOrDebuffIcon {

        public EBuffAndDeBuff eType;
        public Sprite sprite;
    
    }
    [SerializeField]
    public List<BuffOrDebuffIcon> buffOrDebuffIcons;
    private Dictionary<CharacterCombat.AmKinhEntry,ItemUI> dictAmKinhItemUI = new Dictionary<CharacterCombat.AmKinhEntry, ItemUI>();

    [SerializeField] public CharacterCombat char1;
    [SerializeField] public CharacterCombat char2;
    [SerializeField]
    private ItemUI prefabBuffIcon;
    public CharacterCombat currentTurn;
    public CharacterCombat targetTurn => currentTurn == char1 ? char2 : char1;
    public static readonly Dictionary<ELinhKhi, ELinhKhi> linhKhiLienKich = new Dictionary<ELinhKhi, ELinhKhi>()
    {
        { ELinhKhi.Kim, ELinhKhi.Tho },   // trước Kim là Thổ
        { ELinhKhi.Thuy, ELinhKhi.Kim },  // trước Thủy là Kim
        { ELinhKhi.Moc, ELinhKhi.Thuy },  // trước Mộc là Thủy
        { ELinhKhi.Hoa, ELinhKhi.Moc },   // trước Hỏa là Mộc
        { ELinhKhi.Tho, ELinhKhi.Hoa }    // trước Thổ là Hỏa
    };
    public static readonly Dictionary<ELinhKhi, ELinhKhi> TuongSinh = new Dictionary<ELinhKhi, ELinhKhi>()
    {
        { ELinhKhi.Moc,  ELinhKhi.Hoa },   // Mộc sinh Hỏa
        { ELinhKhi.Hoa,  ELinhKhi.Tho },   // Hỏa sinh Thổ
        { ELinhKhi.Tho,  ELinhKhi.Kim },   // Thổ sinh Kim
        { ELinhKhi.Kim,  ELinhKhi.Thuy },  // Kim sinh Thủy
        { ELinhKhi.Thuy, ELinhKhi.Moc }    // Thủy sinh Mộc
    };
    
    [SerializeField]
    private Image progressHpChar1;
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

    public SecureBool IsDelayEffectSkill;

    public Camera cameraEffectSkillDynamic;
    public Camera cameraEffectSkillLock50;
    public Camera cameraEffectSkillLock5;
    [SerializeField] private Button backCombat;
    [SerializeField] private Image bgCombat;

    public Text GetTextDame()
    {
        return prefabTextDame.Spawn(this.transform);
    }
    private void SetUIHpBar(float hp,float maxHp , Image progress , Text txt)
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
            if(item.eType == eBuffAndDeBuff)
            {
                return item.sprite;
            }    
        }
        return null;
    }    
    public void CheckDie()
    {
        CharacterCombat characterDie = null;
        CharacterCombat characterWin = null;

        if (targetTurn.CheckDie())
        {
            characterDie = targetTurn;
            characterWin = currentTurn;
        }
        else if(currentTurn.CheckDie())
        {
            characterDie = currentTurn;
            characterWin = targetTurn;
        }    
        if(characterDie != null)
        {
            Observer.Notify(EventList.Combat.END_COMBAT, characterWin, characterDie);// thằng đầu là thằng win thằng sau là thằng die
            if(characterWin == char1)
            {
                // Fake Data
                Pool.Get<EventRewardPanel>().InitShowCombatReward(new RewardSnapshot {ItemRefs = new(),Money = 20, MoneyVip = 30},"Thắng làm vua, thua làm giặc!");

            }
            else
            {
                
                Pool.Get<EventRewardPanel>().InitShowCombatReward(null,"Thất bại là mẹ thành công!");


            }
            EventList.Combat.RemoveAllCombatObservers();
        } 
            

    }    
    private void Awake()
    {
        Pool.Set(this);
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, EndTurn);
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, PlaySkill);
        Observer.AddObserver(EventList.Combat.ADD_SKILL, SetUISkill);
        Observer.AddObserver(EventList.Combat.ADD_BUFF, SetUIBuff);
        Observer.AddObserver(EventList.Combat.UPDATE_AMKINH_THANTHONG, SetUIAmKinh);

        Observer.AddObserver(EventList.Combat.CHARACTER_ADJUST_HP, SetUIHpBar);
        Observer.AddObserver(EventList.Combat.CHARACTER_ADJUST_HP, CheckDie);

        if (char1 == null) Debug.LogError("Char1 bi Null tai Runtime!");
        if (char2 == null) Debug.LogError("Char2 bi Null tai Runtime!");
        // test 
        //char1.isAutoCastSkill = true;
        var gameConfig = Pool.Get<GameManager>();
        char1.Init(new GameManager.CharCombatInit { charStats= GameManager.ConvertCharServerDataToCharClientData() , eTypeVisual = ETypeVisualChar.Character});
        char2.Init(gameConfig.enemyCombat);
        char2.isAutoCastSkill = true;
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

        SetEventButton(backCombat, () =>
        {
            Pool.Get<PopupConfirmAction>().ShowPopConfirmAction("Xác nhận bỏ chạy khỏi trận chiến này?", () =>
            {
                Pool.Get<GameManager>().BackAllMapFromCombat(ECombatResult.GiveUp);
            });
        });
        bgCombat.sprite = Pool.Get<GameManager>().spriteBGCombat;

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
            return false;

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
        if (thanThong == null || thanThongOldUse == null ) return false;
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
        Observer.Notify(EventList.Combat.PLAY_SKILL, currentTurn,targetTurn, thanThong, new List<LinhKhiTieuTan>());

    }
    public void PlaySkill(params object[] data)
    {
        //if (IsDelayEffectSkill) return;
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
        if (thanThong!= null )
        {


            caster.PlaySkill(thanThong, linhKhiTieuTan);
            CombatStatic.TrienNhieu(target, caster);
            Observer.Notify(EventList.Combat.BEFORE_EXCUTE_SKILL , caster, target);

            if (caster.skillVoHieuHoa.Count > 0)
            {
                target.RemoveAllThanThongImpactAfterPlaySkill(target.specialThanThongImpactAfterPlaySkill.FirstOrDefault(x=>x is AmDuongDong));
                caster.skillVoHieuHoa.Clear();
                return;

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
                }
            }
            if (thanThong is BaseSkill.TuongSinh thanThongTuongSinh && GetETuongQuan(linhKhiTieuTan) == ETuongQuan.TuongSinh)
            {
                thanThongTuongSinh.ExecuteTuongSinh(caster, target);

            }
            if (thanThong is BaseSkill.TuongKhac thanThongTuongKhac && GetETuongQuan(linhKhiTieuTan) == ETuongQuan.TuongKhac)
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
            target.TakeDame(caster,!thanThong.GetEKeywordSkillHave().Contains(BaseSkill.EKeyWord.ChanThucTonThuong));
            if(thanThong.timeDelayTakeDame > 0)
            {
                IsDelayEffectSkill = true;
                parentUIFight.gameObject.SetActive(false);
                DOVirtual.DelayedCall(thanThong.timeDelayTakeDame, () =>
                {
                    IsDelayEffectSkill = false;
                    parentUIFight.gameObject.SetActive(true);

                });
            }    
             
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
    public void EndTurn(params object[] data)
    {
        if (data[2] is List<LinhKhiTieuTan> linhKhiTieuTan)
        {

            TieuTanLinhKhi(linhKhiTieuTan);
        }
       
        CombatStatic.AnhDich(currentTurn, targetTurn);
        CombatStatic.KiemKhi(currentTurn, targetTurn);
        CombatStatic.TrungDoc(targetTurn, currentTurn);
        CombatStatic.ThienKiem(currentTurn, targetTurn);
        CombatStatic.ChayMau(targetTurn, currentTurn);
        currentTurn.EndTurn();
        ExcuteEquipment(currentTurn, targetTurn, EquipmentType.Jewelry);

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
   
    public (bool isHave,ItemUI buffIcon) SetUIEffectStatic(CharacterCombat characterCombat , bool isCreate , Func<ScrollRect,(bool isHave, ItemUI buffIcon)> actionCheckHave,Sprite spriteIcon,string descIcon , int currentValue)
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



                    buffInfor.DoShow();

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
            string desc = $"{BaseSkill.GetColoredText($"[{entry.ThanThong.nameItem}]", "#ffb143")}: {entry.ThanThong.GetDesc(ThanThong.GetDaiCanhGioi(characterCombat.characterStatsTemp.level))}";
            var result = SetUIEffectStatic(characterCombat, remove == -1, (ScrollRect) =>
            {
                if (dictAmKinhItemUI.TryGetValue(entry, out ItemUI item))
                {
                    return (true,item);
                }
                return (false, null);


            },entry.ThanThong.spriteItem, desc, entry.RemainingTurns);
            if (remove != -1)
            {
                if(!result.isHave)
                {
                    dictAmKinhItemUI.Add(entry, result.buffIcon);

                }

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

            string desc = $"{BaseSkill.GetColoredText($"[{thanThong.nameItem}]", "#ffb143")}: {thanThong.GetDesc(daiCanhGioi)}";

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
    private void SetUIBuffEquipment(CharacterCombat characterCombat ,EquipmentClient equipment , EquipmentType equipmentType)
    {
        if (characterCombat)
        {
            string desc = $"{BaseSkill.GetColoredText($"[{equipment.nameEquip}]", "#ffb143")}: {GameManager.GetDescEquipment(equipment.listEffectEquip, equipmentType)}";


            var result = SetUIEffectStatic(characterCombat, false, (scrollRect) =>
            {
               

                return (false, null);


            }, equipment.Item.Icon, desc, 1);

          

        }
    }
    private void SetUIBuffDanDuoc(CharacterCombat characterCombat)
    {
        void SetUIBuffDanDuoc(LinhDan linhDan)
        {
            string desc = $"{BaseSkill.GetColoredText($"[{linhDan.Name}]", "#ffb143")}: {linhDan.Description}";

            var result = SetUIEffectStatic(characterCombat, false, (scrollRect) =>
            {


                return (false, null);


            }, linhDan.Icon, desc ,characterCombat.danDuocCombats.FirstOrDefault(x => x.type.Id == linhDan.Id).count);
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
            string desc = $"{BaseSkill.GetColoredText($"[{ngoDaoCombat.nameItem}]", "#ffb143")}: {ngoDaoCombat.GetDes()}";

            if (ngoDaoCombat)
            {
                var result = SetUIEffectStatic(characterCombat, false, (scrollRect) =>
                {


                    return (false, null);


                }, ngoDaoCombat.spriteItem, desc, 1);
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
    [Button]
    public void NewTurn()
    {
        currentTurn.listDameTaked.Add(0);
        targetTurn.listDameTaked.Add(0);

        CombatStatic.QuyTon(currentTurn,targetTurn);
        CombatStatic.SucThe(currentTurn, targetTurn);
        CombatStatic.ThieuDot(targetTurn, currentTurn);

        ExcuteEquipment(currentTurn, targetTurn, EquipmentType.Armor);
        currentTurn.NewTurn();
        CombatStatic.AmKinhTTNewTurn(currentTurn, targetTurn);

        Dictionary<ELinhKhi, int> dictHapThuKhi = new Dictionary<ELinhKhi, int>();
        if(!currentTurn.IsKietSuc())
        {
            currentTurn.HapThuLinhKhiNewTurn(out dictHapThuKhi);

            

        }
        else
        {
            foreach (var type in new[] { EBuffAndDeBuff.KietSuc, EBuffAndDeBuff.DongKet })
            {
                if (currentTurn.buffAndDebuffCount.TryGetValue(type, out var count))
                    currentTurn.RemoveBuffAndDebuffCount(type, count);
            }

            Observer.Notify(EventList.Combat.END_TURN_COMBAT,currentTurn,targetTurn, new List<LinhKhiTieuTan>());
        }
        // param 0 là bool isPlayer == char1 
        Observer.Notify(EventList.Combat.NEW_TURN_COMBAT, currentTurn,targetTurn,currentTurn == char1? true: false, dictHapThuKhi);
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
        List<ThanThong> thanThongTemp = new List<ThanThong>(currentTurn.thanThongEquip);
        CharacterCombat.PlaySkillData playSkillData = new();

        if (currentTurn.thanThongCurrentUse != null)
        {
            // Tìm item trong list có cùng dữ liệu với thanThongOldUse
            var itemToMove = thanThongTemp.FirstOrDefault(x => x == currentTurn.thanThongCurrentUse.thanThong);

            if (itemToMove != null)
            {
                thanThongTemp.Remove(itemToMove); // Xóa khỏi vị trí hiện tại
                thanThongTemp.Add(itemToMove);    // Thêm lại vào cuối list
            }
            // check skill play ưu tiên liên kích
            foreach (var item in thanThongTemp)
            {
                if( item is BaseSkill.LienKich && IsLienKich(item,currentTurn.thanThongCurrentUse.thanThong))
                {
                    List<LinhKhiTieuTan> linhKhiTieuTanTemp = item.GetLinhKhiTieuTanIfPossible(currentTurn);
                    if(linhKhiTieuTanTemp != null && linhKhiTieuTanTemp.Count >0 )
                    {
                        return new CharacterCombat.PlaySkillData {thanThong = item,linhKhiTieuTan = linhKhiTieuTanTemp };
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
            if(linhKhiTieuTanTemp != null && linhKhiTieuTanTemp.Count<=0)
            {
                linhKhiTieuTanTemp = item.GetLinhKhiTieuTanIfPossible(currentTurn);
            }
            if (linhKhiTieuTanTemp != null&&linhKhiTieuTanTemp.Count >0)
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
            if(!currentTurn.isAutoCastSkill)
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
            if (playSkillData != null&&playSkillData.linhKhiTieuTan != null&&playSkillData.thanThong.IsCanUseLinhKhiRequire(currentTurn))
            {
                Observer.Notify(EventList.Combat.PLAY_SKILL, currentTurn, targetTurn, playSkillData.thanThong, playSkillData.linhKhiTieuTan);

            }
            else
            {
                Observer.Notify(EventList.Combat.END_TURN_COMBAT, currentTurn, targetTurn, currentTurn.GetLinhKhiTieuTanForTotalLinhKhi());
                break;
            }  
            
                
        } 
            

    }
    private void StartTurn()
    {
        currentTurn = char1.GetToDoBay() >= char2.GetToDoBay() ? char1 : char2;
        StartEquipment(currentTurn);
        StartEquipment(targetTurn);

    }

    void Start()
    {
        StartTurn();
        NewTurn();
    }
  
    public void ExcuteWeapon()
    {
        if(!currentTurn.isCooldownWeapon)
        {
            ExcuteEquipment(currentTurn,targetTurn,EquipmentType.Weapon);
            currentTurn.isCooldownWeapon = true;
            if(currentTurn.currentVuKhiVisual!=null)
            {
                currentTurn.currentVuKhiVisual.ActiveAttack();

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
        foreach (var item in equipment.listEffectEquip)
        {
            ConvertEffectEquipmentToBuffAction(item)?.Invoke(caster, target);

        }
    }
    // Update is called once per frame
    public static Action<CharacterCombat, CharacterCombat> ConvertEffectEquipmentToBuffAction(EquipmentClient.EffectClient effect)
    {
        switch (effect.Type)
        {
            case EffectType.Bui:

                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Tran, effect.Value);
                };
            case EffectType.Cat:
                return (caster, target) =>
                {
                    target.AddBuffAndDebuffCount(EBuffAndDeBuff.Cat, effect.Value);
                };
            case EffectType.ChanHoa:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanHoa, effect.Value);
                };
            case EffectType.ChanKhi:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanKhi, effect.Value);
                };
            case EffectType.ChanKiem:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanKiem, effect.Value);
                };
            case EffectType.ChanKim:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanKim, effect.Value);
                };
            case EffectType.ChanMoc:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanMoc, effect.Value);
                };
            case EffectType.ChanThan:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanThan, effect.Value);
                };
            case EffectType.ChanTho:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanTho, effect.Value);
                };
            case EffectType.ChanThuy:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanThuy, effect.Value);
                };
            case EffectType.ChayMau:
                return (caster, target) =>
                {
                    target.AddBuffAndDebuffCount(EBuffAndDeBuff.ChayMau, effect.Value);
                };
            case EffectType.DeTonThuong:
                return (caster, target) =>
                {
                    target.AddBuffAndDebuffCount(EBuffAndDeBuff.DeTonThuong, effect.Value);
                };
            case EffectType.Diem:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Diem, effect.Value);
                };
            case EffectType.GiamTonThuong:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.GiamTonThuong, effect.Value);
                };
            case EffectType.HapThuHoaLinhKhi:
                return (caster, target) =>
                {
                    caster.HapThuLinhKhi(ELinhKhi.Hoa, effect.Value);
                };
            case EffectType.HapThuKimLinhKhi:
                return (caster, target) =>
                {
                    caster.HapThuLinhKhi(ELinhKhi.Kim, effect.Value);
                };
            case EffectType.HapThuLinhKhi:
                return (caster, target) =>
                {
                    caster.HapThuLinhKhi(effect.Value);

                };
            case EffectType.HapThuMocLinhKhi:
                return (caster, target) =>
                {
                    caster.HapThuLinhKhi(ELinhKhi.Moc, effect.Value);
                };
            case EffectType.HapThuThoLinhKhi:
                return (caster, target) =>
                {
                    caster.HapThuLinhKhi(ELinhKhi.Tho, effect.Value);
                };
            case EffectType.HapThuThuyLinhKhi:
                return (caster, target) =>
                {
                    caster.HapThuLinhKhi(ELinhKhi.Thuy, effect.Value);
                };
            case EffectType.HoaDiem:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoaDiem, effect.Value);
                };
            case EffectType.HoaTrong:
                break;
            case EffectType.HoTrao:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, effect.Value);
                };
            case EffectType.KhoiPhucGiaTriSinhMenh:
                return (caster, target) =>
                {
                    caster.Healing(effect.Value);
                };
            case EffectType.KhoiPhucGiaTriSinhMenhDauLuot:
                return (caster, target) =>
                {
                    caster.Healing(effect.Value);
                };
            case EffectType.KiemKhi:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, effect.Value);
                };
            case EffectType.KimTrong:
                break;
            case EffectType.MocTrong:
                break;
            case EffectType.NguHoa:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguHoa, effect.Value);
                };
            case EffectType.NguHoaHiep:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguHoaHiep, effect.Value);
                };
            case EffectType.NguKhiHiep:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguKhiHiep, effect.Value);
                };
            case EffectType.NguKiemHiep:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguKiemHiep, effect.Value);
                };
            case EffectType.NguKim:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguKim, effect.Value);
                };
            case EffectType.NguKimHiep:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguKimHiep, effect.Value);
                };
            case EffectType.NguMoc:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguMoc, effect.Value);
                };
            case EffectType.NguMocHiep:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguMocHiep, effect.Value);
                };
            case EffectType.NgungThan:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NgungThan, effect.Value);
                };
            case EffectType.NguThanHiep:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguThanThucHiep, effect.Value);
                };
            case EffectType.NguTho:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguTho, effect.Value);
                };
            case EffectType.NguThoHiep:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguThoHiep, effect.Value);
                };
            case EffectType.NguThuy:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguThuy, effect.Value);
                };
            case EffectType.NguThuyHiep:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguThuyHiep, effect.Value);
                };
            case EffectType.Nhanh:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TangToc, effect.Value);
                };
            case EffectType.PhanTonThuong:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.PhanTonThuong, effect.Value);
                };
            case EffectType.PhaThe:
                return (caster, target) =>
                {
                    target.AddBuffAndDebuffCount(EBuffAndDeBuff.PhaThe, effect.Value);
                };
            case EffectType.QuanQuanh:
                return (caster, target) =>
                {
                    target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu, effect.Value);
                };
            case EffectType.SucThe:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.SucThe, effect.Value);
                };
            case EffectType.SuongGia:
                return (caster, target) =>
                {
                    target.AddBuffAndDebuffCount(EBuffAndDeBuff.SuongGia, effect.Value);
                };
            case EffectType.TaoThanhTonThuong:
                return (caster, target) =>
                {
                    target.TakeDameByStatic(caster, new List<int> { effect.Value }, false);
                };
            case EffectType.TaoThanhXLanXDiemThuongTon:
                return (caster, target) =>
                {

                    for (int i = 0; i < effect.Value; i++)
                    {
                        target.TakeDameByStatic(caster, new List<int> { effect.Value }, false);

                    }
                };
            case EffectType.ThanThuong:

                return (caster, target) =>
                {
                    target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThanThuong, effect.Value);
                };
            case EffectType.ThieuDot:
                return (caster, target) =>
                {
                    target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, effect.Value);
                };
            case EffectType.ThoTrong:
                break;
            case EffectType.ThuyTrong:
                break;
            case EffectType.TichDoc:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TichDoc, effect.Value);
                };
            case EffectType.TichHoa:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TichHoa, effect.Value);
                };
            case EffectType.TichKim:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TichKim, effect.Value);
                };
            case EffectType.TichMoc:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TichMoc, effect.Value);
                };
            case EffectType.TichThuy:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TichThuy, effect.Value);
                };
            case EffectType.TriLieuLieu:
                return (caster, target) =>
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TriLieu, effect.Value);
                };
            case EffectType.TroiDu:
                return (caster, target) =>
                {
                    target.AddBuffAndDebuffCount(EBuffAndDeBuff.TroiChan, effect.Value);
                };
            case EffectType.TrungDoc:
                return (caster, target) =>
                {
                    target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrungDoc, effect.Value);
                };
            default:
                break;
        }
        return null;
    }
    private void OnDestroy()
    {
        EventList.Combat.RemoveAllCombatObservers();
    }
}
