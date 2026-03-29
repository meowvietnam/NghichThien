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
            Pool.Get<ChatPanel>().gameObject.SetActive(true);
        });
        SetEventButton(headBtn, () =>
        {
            Pool.Get<TabPanel>().gameObject.SetActive(true);
        });
    }
   
    private void UpdatePlayerStats()
    {
        hpText.text = $"{Client.Instance.CharacterData.Stats.Hp}/{Client.Instance.CharacterData.Stats.SinhMenh}";
        Slider hpSlider = hpText.transform.parent.GetComponent<Slider>();
        hpSlider.value = (float)Client.Instance.CharacterData.Stats.Hp / Client.Instance.CharacterData.Stats.SinhMenh;

        tuviText.text = $"{Client.Instance.CharacterData.CurrentExp}/{GameConfig.GetTuviRequired(Client.Instance.CharacterData.Level)}";
        Slider expSlider = tuviText.transform.parent.GetComponent<Slider>();
        expSlider.value = (float)Client.Instance.CharacterData.CurrentExp / GameConfig.GetTuviRequired(Client.Instance.CharacterData.Level);

        iconLevel.sprite = listSpriteIconLevel[Client.Instance.CharacterData.Level-1];
        iconLevel.SetNativeSize();
        nameText.text = Client.Instance.CharacterData.Name;
        characterGraphic.SetGender(Client.Instance.CharacterData.Gender);
        characterGraphic.SetSkin(Client.Instance.CharacterData.Skins);
    }
}
