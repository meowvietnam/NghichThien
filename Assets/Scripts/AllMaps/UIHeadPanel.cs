using AllMaps;
using Base;
using LongDH.LongDHModules;
using UnityEngine;
using UnityEngine.UI;

public class UIHeadPanel : BaseBehaviour
{
    [SerializeField] private Button headBtn;
    [SerializeField] private Button chatBtn;
    [SerializeField] private Button tutorialBtn;
    [SerializeField] private Image iconLevel;
    [SerializeField] private Text hpText;
    [SerializeField] private Text tuviText;
    [SerializeField] private Text nameText;
    [SerializeField] private Sprite[] listSpriteIconLevel;


    private void Awake()
    {
        Pool.Set(this);
        Observer.AddObserver(EventList.GamePlay.UPDATE_PLAYER_STATS, UpdatePlayerStats);
        SetEventButton(chatBtn, () =>
        {
            Pool.Get<ChatPanel>().gameObject.SetActive(true);
        });
    }
    private void UpdatePlayerStats()
    {
        var player = Pool.Get<Player>();
        hpText.text = $"{player.characterStatsBase.hp.Value}/{player.characterStatsBase.hpMax.Value}";
        Slider hpSlider = hpText.transform.parent.GetComponent<Slider>();
        hpSlider.value = (float)player.characterStatsBase.hp.Value/player.characterStatsBase.hpMax.Value;

        tuviText.text = $"{player.characterStatsBase.currentExp.Value}/{ValueConstants.GetTuviRequired(player.characterStatsBase.level).Value}";
        Slider expSlider = tuviText.transform.parent.GetComponent<Slider>();
        expSlider.value = (float)player.characterStatsBase.currentExp.Value / ValueConstants.GetTuviRequired(player.characterStatsBase.level).Value;

        iconLevel.sprite = listSpriteIconLevel[player.characterStatsBase.level-1];
        iconLevel.SetNativeSize();
    }
}
