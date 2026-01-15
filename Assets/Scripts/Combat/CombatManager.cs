using LongDH.LongDHModules;
using LongDH.Script.ObjectPool;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class LinhKhiTieuTan
{
    public ELinhKhi eLinhKhi;
    public int count;
}
public enum ETuongQuan
{
    TuongSinh,
    TuongKhac,
    None,
}

public class CombatManager : MonoBehaviour
{
    [System.Serializable]   
    public class BuffOrDebuffIcon {

        public EBuffAndDeBuff eType;
        public Sprite sprite;
    
    }
    [SerializeField]
    private List<BuffOrDebuffIcon> buffOrDebuffIcons;
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

    [SerializeField] private Text prefabTextDame;
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
        SetUIHpBar(char1.characterStatsCurrent.hp, char1.characterStats.hp, progressHpChar1, textHpChar1);
        SetUIHpBar(char2.characterStatsCurrent.hp, char2.characterStats.hp, progressHpChar2, textHpChar2);

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
  
    private void Awake()
    {
       
        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, EndTurn);
        Observer.AddObserver(EventList.GamePlay.PLAY_SKILL, PlaySkill);
        Observer.AddObserver(EventList.GamePlay.ADD_BUFF, SetUIBuff);
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME, SetUIHpBar);
        char1.Init(new CharacterStats()
        {
            level = 9,
            percentKim = 60,
            percentMoc = 10,
            percentThuy = 10,
            percentHoa = 10,
            percentTho = 10,
            hp = 200,
            tocDoBay = 20,

        });
        char2.Init(new CharacterStats()
        {
            level = 9,
            percentKim = 60,
            percentMoc = 10,
            percentThuy = 10,
            percentHoa = 10,
            percentTho = 10,
            hp = 200,
            tocDoBay = 10,
        });
        char2.currentCharGender.transform.localEulerAngles = new Vector3(0, 180, 0);
        SetUIHpBar();
    }
    
    public static bool IsTuongDong(List<LinhKhiTieuTan> linhKhiTieuTan)
    {
        // Lấy danh sách các hệ có count > 0
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
        // Lấy danh sách các hệ có mặt
        List<ELinhKhi> list = linhKhiTieuTan
                              .Where(x => x.count > 0)
                              .Select(x => x.eLinhKhi)
                              .ToList();


        if (list.Count == 1)
            return ETuongQuan.None;

        if (list.Count >= 4)
            return ETuongQuan.TuongKhac;

        // totalCount == 2 || 3 → cần kiểm tra tương sinh
        bool allSinh = true;

        for (int i = 0; i < list.Count; i++)
        {
            for (int j = i + 1; j < list.Count; j++)
            {
                ELinhKhi a = list[i];
                ELinhKhi b = list[j];

                bool sinh =
                    (TuongSinh.ContainsKey(a) && TuongSinh[a] == b) ||
                    (TuongSinh.ContainsKey(b) && TuongSinh[b] == a);

                if (!sinh)
                {
                    allSinh = false;
                    break;
                }
            }
            if (!allSinh) break;
        }

        return allSinh ? ETuongQuan.TuongSinh : ETuongQuan.TuongKhac;
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
    public void PlaySkill(params object[] data)
    {
        CharacterCombat caster = data[0] as CharacterCombat;
        CharacterCombat target = data[1] as CharacterCombat;

        ThanThong thanThong = data[2] as ThanThong;
        List<LinhKhiTieuTan> linhKhiTieuTan = data[3] as List<LinhKhiTieuTan>;
        ThanThong thanThongOldUse = caster.thanThongOldUse;
        if (thanThong!= null)
        {
            caster.PlaySkill(thanThong,linhKhiTieuTan);
            thanThong.Execute(caster, target);
            if (thanThong is BaseSkill.LienKich thanThongLienKich && thanThongOldUse != null)
            {
                if (IsLienKich(thanThong , thanThongOldUse))
                {
                    Debug.Log("Liên kích!");
                    thanThongLienKich.ExecuteLienKich(caster, target);
                }
            }
            if(thanThong is BaseSkill.TuongSinh thanThongTuongSinh && GetETuongQuan(linhKhiTieuTan) == ETuongQuan.TuongSinh)
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
            caster.SetThanThongOldUse(thanThong);
            target.TakeDame();
        }
        if (linhKhiTieuTan != null)
        {
            TieuTanLinhKhi(linhKhiTieuTan);

            
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
    public void EndTurn(params object[] data)
    {
        if (data[2] is List<LinhKhiTieuTan> linhKhiTieuTan)
        {
            TieuTanLinhKhi(linhKhiTieuTan);
        }
        if (currentTurn.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out int kiemKhi))
        {
            targetTurn.TakeDameCaculate(currentTurn, new List<int> { kiemKhi });
            targetTurn.TakeDame();

        }
        char1.EndTurn();
        char2.EndTurn();
        if (currentTurn == char1)
        {
            currentTurn = char2;
        } 
        else
        {
            currentTurn = char1;
        }
        NewTurn();
    }
    public void SetUIBuff(params object[] data)
    {
        if (data[0] is CharacterCombat characterCombat)
        {
            EBuffAndDeBuff buffType = (EBuffAndDeBuff)data[1];
            int currentValue = (int)data[2];
            ItemUI buffIcon = null;
            ScrollRect scrollRect = null;
            if (characterCombat == char1)
            {
                scrollRect = statusChar1;

            }
            else
            {
                scrollRect = statusChar2;


            }
            bool isHave = false;
            for (int i = 0; i < scrollRect.content.transform.childCount; i++)
            {
                ItemUI itemUI = scrollRect.content.transform.GetChild(i).GetComponent<ItemUI>();
                if (itemUI.icon.sprite == GetSpriteBuffIcon(buffType)) // cos toon tai
                {
                    isHave = true;
                    buffIcon = itemUI;
                    break;
                }

            }
            if (currentValue <= 0)
            {
                if(isHave)
                {
                    buffIcon.Recycle();
                }    
            }
            else
            {
                
                if (!isHave)
                {
                    buffIcon = prefabBuffIcon.Spawn(scrollRect.content.transform);
                    buffIcon.transform.localScale = Vector3.one;
                    buffIcon.InitIcon(GetSpriteBuffIcon(buffType), null);
                }
                
                buffIcon.AddCount(currentValue - buffIcon.count);
            }
          
        }
      
    }
    [Button]
    public void NewTurn()
    {
        if (currentTurn.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SucThe, out int sucThe))
        {
            targetTurn.TakeDameCaculate(currentTurn, new List<int> { sucThe });
            targetTurn.TakeDame();

        }
        char1.NewTurn();
        char2.NewTurn();
       
        
        if(!currentTurn.IsKietSuc())
        {
            currentTurn.HapThuLinhKhi();

        }
        else
        {
            currentTurn.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KietSuc, currentTurn.buffAndDebuffCount[EBuffAndDeBuff.KietSuc]);
            Observer.Notify(EventList.GamePlay.END_TURN_COMBAT,currentTurn,targetTurn, new List<LinhKhiTieuTan>());
        }
        // param 0 là bool isPlayer == char1 
        Observer.Notify(EventList.GamePlay.NEW_TURN_COMBAT, currentTurn,targetTurn,currentTurn == char1? true: false);


    }

    private void StartTurn()
    {
        currentTurn = char1.characterStatsCurrent.tocDoBay >= char2.characterStatsCurrent.tocDoBay ? char1 : char2;

    }    
    void Start()
    {
        StartTurn();
        NewTurn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
