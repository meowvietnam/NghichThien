using LongDH.LongDHModules;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class LinhKhiTieuTan
{
    public ELinhKhi eLinhKhi;
    public int count;
}
public enum ETuongQuan
{
    TuongSinh,
    TuongKhac,
    None
}
public class CombatManager : MonoBehaviour
{
    [SerializeField] private CharacterCombat char1;
    [SerializeField] private CharacterCombat char2;

    public CharacterCombat currentTurn;
    public CharacterCombat target => currentTurn == char1 ? char2 : char1;
    static readonly Dictionary<ELinhKhi, ELinhKhi> linhKhiLienKich = new Dictionary<ELinhKhi, ELinhKhi>()
    {
        { ELinhKhi.Kim, ELinhKhi.Tho },   // trước Kim là Thổ
        { ELinhKhi.Thuy, ELinhKhi.Kim },  // trước Thủy là Kim
        { ELinhKhi.Moc, ELinhKhi.Thuy },  // trước Mộc là Thủy
        { ELinhKhi.Hoa, ELinhKhi.Moc },   // trước Hỏa là Mộc
        { ELinhKhi.Tho, ELinhKhi.Hoa }    // trước Thổ là Hỏa
    };
    static readonly Dictionary<ELinhKhi, ELinhKhi> TuongSinh = new Dictionary<ELinhKhi, ELinhKhi>()
    {
        { ELinhKhi.Moc,  ELinhKhi.Hoa },   // Mộc sinh Hỏa
        { ELinhKhi.Hoa,  ELinhKhi.Tho },   // Hỏa sinh Thổ
        { ELinhKhi.Tho,  ELinhKhi.Kim },   // Thổ sinh Kim
        { ELinhKhi.Kim,  ELinhKhi.Thuy },  // Kim sinh Thủy
        { ELinhKhi.Thuy, ELinhKhi.Moc }    // Thủy sinh Mộc
    };
    
    private ThanThong thanThongOldUse => currentTurn.thanThongOldUse;
    private void Awake()
    {
        char1.Init(new CharacterStats()
        {
            percentKim = 60,
            percentMoc = 10,
            percentThuy = 10,
            percentHoa = 10,
            percentTho = 10,

        });
        char2.Init(new CharacterStats());
        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, EndTurn);
        Observer.AddObserver(EventList.GamePlay.PLAY_SKILL, PlaySkill);

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
        ThanThong thanThong = data[0] as ThanThong;
        List<LinhKhiTieuTan> linhKhiTieuTan = data[1] as List<LinhKhiTieuTan>;
        if (thanThong!= null)
        {
            thanThong.Execute(currentTurn,target);
            if (thanThong is BaseSkill.LienKich thanThongLienKich && thanThongOldUse != null)
            {
                if (IsLienKich(thanThong , thanThongOldUse))
                {
                    Debug.Log("Liên kích!");
                    thanThongLienKich.ExecuteLienKich(currentTurn, target);
                }
            }
            if(thanThong is BaseSkill.TuongSinh thanThongTuongSinh && GetETuongQuan(linhKhiTieuTan) == ETuongQuan.TuongSinh)
            {
                thanThongTuongSinh.ExecuteTuongSinh(currentTurn, target);

            }
            if (thanThong is BaseSkill.TuongKhac thanThongTuongKhac && GetETuongQuan(linhKhiTieuTan) == ETuongQuan.TuongKhac)
            {
                thanThongTuongKhac.ExecuteTuongKhac(currentTurn, target);

            }
            currentTurn.thanThongOldUse = thanThong;
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
        if (data[0] is List<LinhKhiTieuTan> linhKhiTieuTan)
        {
            TieuTanLinhKhi(linhKhiTieuTan);
        }
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
    [Button]
    public void NewTurn()
    {
        currentTurn.HapThuLinhKhi();
        // param 0 là bool isPlayer == char1 
        Observer.Notify(EventList.GamePlay.NEW_TURN_COMBAT, currentTurn == char1? true: false,currentTurn);


    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
