using AllMaps;
using Base;
using CreateCharacter;
using LongDH.LongDHModules;
using LongDHModules;
using Network;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class UIHeadPanel : PanelGroup
{
    [SerializeField] private Button headBtn;
    [SerializeField] private Button tutorialBtn;
    [SerializeField] private Image iconLevel;
    [SerializeField] private Text hpText;
    [SerializeField] private Text tuviText;
    [SerializeField] private Text nameText;
    [SerializeField] private Sprite[] listSpriteIconLevel;
    [SerializeField] private CharacterGraphic characterGraphic;
    public MailButton mailBtn;
    public override void Init()
    {
        base.Init();
        Observer.AddObserver(EventList.GamePlay.UPDATE_PLAYER_STATS, UpdatePlayerStats);

        SetEventButton(headBtn, () =>
        {
            var tabPanel = Pool.Get<TabPanel>();
            tabPanel.gameObject.SetActive(true);
            tabPanel.transform.SetAsLastSibling();
        });
    }
   
    private void UpdatePlayerStats()
    {
        var charData = Client.Instance.CharacterData;
        var progression = charData.Progression;
        var stats = charData.Stats;
        var gameConfig = Pool.Get<GameManager>();
        hpText.text = $"{stats.Hp}/{stats.SinhMenh}";
        Slider hpSlider = hpText.transform.parent.GetComponent<Slider>();
        hpSlider.value = (float)stats.Hp / stats.SinhMenh;

        tuviText.text = $"{progression.CurrentExp}/{gameConfig.inforLevels[progression.Level-1].tuviRequireNextLevel}";
        Slider expSlider = tuviText.transform.parent.GetComponent<Slider>();
        expSlider.value = (float)progression.CurrentExp / gameConfig.inforLevels[progression.Level-1].tuviRequireNextLevel;

        iconLevel.sprite = listSpriteIconLevel[progression.Level-1];
        iconLevel.SetNativeSize();
        nameText.text = charData.Name;
        characterGraphic.SetGender(charData.Gender);
        characterGraphic.SetSkin(charData.Appearance.Skins,charData.Appearance.Colors);
    }

    private void OnDestroy()
    {
        Observer.RemoveObserver(EventList.GamePlay.UPDATE_PLAYER_STATS, UpdatePlayerStats);
    }
}
