using AllMaps;
using Base;
using CreateCharacter;
using LongDH.LongDHModules;
using Network;
using UnityEngine;
using UnityEngine.UI;

public class UIHeadPanel : PanelGroup
{
    [SerializeField] private Button headBtn;
    [SerializeField] private Button chatBtn;
    [SerializeField] private Button tutorialBtn;
    [SerializeField] private Image iconLevel;
    [SerializeField] private Text hpText;
    [SerializeField] private Text tuviText;
    [SerializeField] private Text nameText;
    [SerializeField] private Sprite[] listSpriteIconLevel;
    [SerializeField] private CharacterGraphic characterGraphic;

    public override void Init()
    {
        base.Init();
        Observer.AddObserver(EventList.GamePlay.UPDATE_PLAYER_STATS, UpdatePlayerStats);
        SetEventButton(chatBtn, () =>
        {
            var ChatPanel = Pool.Get<ChatPanel>();
            ChatPanel.gameObject.SetActive(true);
            ChatPanel.transform.SetAsLastSibling();

        });
        SetEventButton(headBtn, () =>
        {
            var tabPanel = Pool.Get<TabPanel>();
            tabPanel.gameObject.SetActive(true);
            tabPanel.transform.SetAsLastSibling();
        });
    }
   
    private void UpdatePlayerStats()
    {
        var gameConfig = Pool.Get<GameConfig>();
        hpText.text = $"{Client.Instance.CharacterData.Stats.Hp}/{Client.Instance.CharacterData.Stats.SinhMenh}";
        Slider hpSlider = hpText.transform.parent.GetComponent<Slider>();
        hpSlider.value = (float)Client.Instance.CharacterData.Stats.Hp / Client.Instance.CharacterData.Stats.SinhMenh;

        tuviText.text = $"{Client.Instance.CharacterData.CurrentExp}/{gameConfig.inforLevels[Client.Instance.CharacterData.Level-1].tuviRequireNextLevel}";
        Slider expSlider = tuviText.transform.parent.GetComponent<Slider>();
        expSlider.value = (float)Client.Instance.CharacterData.CurrentExp / gameConfig.inforLevels[Client.Instance.CharacterData.Level-1].tuviRequireNextLevel;

        iconLevel.sprite = listSpriteIconLevel[Client.Instance.CharacterData.Level-1];
        iconLevel.SetNativeSize();
        nameText.text = Client.Instance.CharacterData.Name;
        characterGraphic.SetGender(Client.Instance.CharacterData.Gender);
        characterGraphic.SetSkin(Client.Instance.CharacterData.Skins);
    }
}
