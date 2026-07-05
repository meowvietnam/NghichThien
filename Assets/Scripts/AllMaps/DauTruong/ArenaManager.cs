using CreateCharacter;
using DG.Tweening;
using LongDH.LongDHModules;
using LongDHModules;
using Network;
using Network.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaManager : BaseBehaviour , INetworkEvent.ILoadCharCombatArena
{
    [SerializeField] private Image iconRank;
    [SerializeField] private Image iconRankName;
    [SerializeField] private Button btnStart;
    [SerializeField] public Sprite[] spriteRefRank;
    [SerializeField] public Sprite[] spriteRefRankName;
    [SerializeField] private int pointUser => Client.Instance.CharacterData.ArenaManager.Points; // sau này chuyển cái này thành của player
    public ETypeArenaRank CurrentTypeRank => GetArenaRankWithPoint(pointUser);
    public static ArenaManager Instance;
    [SerializeField] private PrepareElementArenaCombat userUI;
    [SerializeField] private PrepareElementArenaCombat enemyUI;
    [SerializeField] private Transform parentPrepareCombat => userUI.transform.parent;
    [SerializeField]
    private Button btnBackScene;
    [SerializeField] private Sprite spriteCombat;
    [SerializeField] private Image progressRank;
    [SerializeField] private Image progressBar;
    [SerializeField] private Text txtPoint;
    private void SetProgress()
    {
        if(CurrentTypeRank == ETypeArenaRank.ThanhGiai)
        {
            progressRank.gameObject.SetActive(false);
            txtPoint.text = pointUser.ToString();
            progressBar.fillAmount = 1f;
        }   
        else
        {
            ETypeArenaRank nextRank = (ETypeArenaRank)((int)CurrentTypeRank+1);
            int valueNextRank = TypeRankArenaWithValue[nextRank];
            progressRank.gameObject.SetActive(true);
            progressRank.sprite= spriteRefRank[(int)nextRank-1];
            txtPoint.text =$"{pointUser}/{valueNextRank}";
            progressBar.fillAmount = (float)(pointUser/valueNextRank);
        }    
    }    
    private void Awake()
    {
        Instance = this;
    }
    public static ETypeArenaRank GetArenaRankWithPoint(int pointUser)
    {
        // Duyệt qua từng dòng trong Dictionary (đã được định nghĩa sẵn từ cao xuống thấp)
        foreach (var rank in TypeRankArenaWithValue)
        {
            // Nếu điểm của User lớn hơn hoặc bằng mốc điểm của Rank này
            if (pointUser >= rank.Value)
            {
                return rank.Key; // Trả về Rank cao nhất thỏa mãn và thoát hàm
            }
        }

        // Trường hợp điểm nhỏ hơn cả mốc thấp nhất (ví dụ: điểm = 0)
        // Bạn có thể trả về HoangGiai hoặc một mức mặc định tùy logic game
        return ETypeArenaRank.None;
    }
    public static Dictionary<ETypeArenaRank, int> TypeRankArenaWithValue = new Dictionary<ETypeArenaRank, int>
    {
        {ETypeArenaRank.ThanhGiai,2100},
        {ETypeArenaRank.ThanGiai,1500},
        {ETypeArenaRank.ThienGiai,1100},
        {ETypeArenaRank.DiaGiai,800},
        {ETypeArenaRank.HuyenGiai,400},
        {ETypeArenaRank.HoangGiai,1},

    };
    public static Dictionary<ETypeArenaRank, int> TypeRankArenaWithReward = new Dictionary<ETypeArenaRank, int>
    {
        {ETypeArenaRank.ThanhGiai,3000},
        {ETypeArenaRank.ThanGiai,2000},
        {ETypeArenaRank.ThienGiai,1000},
        {ETypeArenaRank.DiaGiai,500},
        {ETypeArenaRank.HuyenGiai,300},
        {ETypeArenaRank.HoangGiai,100},

    };
    private void Start()
    {
        SetEventButton(btnBackScene, () =>
        {
            Observer.Notify(EventList.GamePlay.BACK_SCENE);
        });
        SetEventButton(btnStart, () =>
        {
            Observer.Notify(EventList.Network.LOAD_CHAR_COMBAT_ARENA, this);
        });
        var currentRank = CurrentTypeRank;
        if(currentRank == ETypeArenaRank.None)
        {
            iconRank.gameObject.SetActive(false);
            iconRankName.gameObject.SetActive(false);
        }
        else
        {
            
            iconRank.gameObject.SetActive(true);
            iconRankName.gameObject.SetActive(true);
            iconRank.sprite = spriteRefRank[(int)currentRank - 1];
            iconRank.SetNativeSize();
            iconRankName.sprite = spriteRefRankName[(int)currentRank - 1];
            iconRankName.SetNativeSize();
        }
        Pool.Get<UIManager>().panelGroup.SetActive(false);
        Pool.Get<UIManager>().commonMask.gameObject.SetActive(false);
        Pool.Get<UIManager>().ListIcon.gameObject.SetActive(false);
        SetProgress();
    }
    public void SuccessLoadCharCombatArena(CharacterSnapshot user, CharacterSnapshot enemy)
    {
        var gameManager = Pool.Get<GameManager>();
        gameManager.GoToPrepareCombatArena(user, enemy);
        userUI.InitVisual(gameManager.user.charStats);
        enemyUI.InitVisual(gameManager.enemyCombat.charStats);
        parentPrepareCombat.gameObject.SetActive(true);
        gameManager.SetBGCombat(spriteCombat);
        // show anim hiệu ứng chuẩn bị đánh nhau 
        // delay vài giây xong thì vào combat
        DOVirtual.DelayedCall(2f, () =>
        {
            gameManager.GoToCombatArena();
        });

    }

    public void FailLoadCharCombatArena(string message)
    {
        Debug.Log(message);
    }
}
