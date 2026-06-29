using CreateCharacter;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class RewardRankDauTruong : BaseBehaviour
{
    [SerializeField] private Button btnReward;
    [SerializeField] private Button btnNotReward;
    [SerializeField] private Image iconRank;
    [SerializeField] private Image iconRankName;
    [SerializeField] private Text rewardTienNgocText;
    [SerializeField] private Sprite[] spriteRefRank => ArenaManager.Instance.spriteRefRank;
    [SerializeField] private Sprite[] spriteRefRankName => ArenaManager.Instance.spriteRefRankName;

    [SerializeField] private ETypeArenaRank eTypeArenaRank;

    void Start()
    {
        InitVisual();
        if(ArenaManager.Instance.CurrentTypeRank >= eTypeArenaRank)
        {
            SetCanReward(true);
        }
    }
    [Button]
    public void InitVisual()
    {
        SetEventButton(btnReward, () =>
        {

        });
        SetEventButton(btnNotReward, () =>
        {
            NotifyGroup.Instance.Show("Chưa đủ điều kiện nhận thưởng");
        });
        iconRank.sprite = spriteRefRank[(int)eTypeArenaRank-1];
        iconRank.SetNativeSize();
        iconRankName.sprite = spriteRefRankName[(int)eTypeArenaRank - 1];
        iconRankName.SetNativeSize();
        rewardTienNgocText.text = ArenaManager.TypeRankArenaWithReward[eTypeArenaRank].ToString();
        SetCanReward(false);
    }
    public void SetCanReward(bool isCan)
    {
        btnReward.gameObject.SetActive(isCan);
        btnNotReward.gameObject.SetActive(!isCan);
    }    
}
