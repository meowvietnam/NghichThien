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

    /// <summary>Nút hình nhân vật (góc trái) mở bảng nhân vật (TabPanel).
    /// Public để hand-tut khoanh sáng được; UIHeadPanel là PanelGroup nên lấy qua
    /// Pool.Get&lt;UIHeadPanel&gt;() từ bất kỳ scene nào.</summary>
    public Button HeadBtn => headBtn;
    [SerializeField] private Button tutorialBtn;
    [SerializeField] private Image iconLevel;
    [SerializeField] private Text hpText;
    [SerializeField] private Text tuviText;
    [SerializeField] private Text nameText;
    [SerializeField] private Sprite[] listSpriteIconLevel;
    [SerializeField] private CharacterGraphic characterGraphic;
    public MailButton mailBtn;

    /// <summary>
    /// Nguồn HP hiển thị thay cho HP thường (vd. HP chiến trong scene Tông môn chiến).
    /// Trả null thì hiển thị HP thường của nhân vật.
    /// </summary>
    private System.Func<(int Hp, int HpMax, string Suffix)?> hpOverride;

    public void SetHpOverride(System.Func<(int Hp, int HpMax, string Suffix)?> provider)
    {
        hpOverride = provider;
        RefreshHp();
    }

    /// <summary>Cập nhật riêng thanh HP (gọi định kỳ khi đang dùng HP thay thế).</summary>
    public void RefreshHp()
    {
        if (hpText == null) return;
        var custom = hpOverride?.Invoke();
        int hp, max;
        string suffix = string.Empty;
        if (custom.HasValue)
        {
            hp = custom.Value.Hp;
            max = custom.Value.HpMax;
            suffix = custom.Value.Suffix ?? string.Empty;
        }
        else
        {
            var stats = Client.Instance?.CharacterData?.Stats;
            if (stats == null) return;
            hp = stats.Hp;
            max = stats.SinhMenh;
        }

        hpText.text = $"{hp}/{max}{suffix}";
        var hpSlider = hpText.transform.parent.GetComponent<Slider>();
        if (hpSlider != null) hpSlider.value = max > 0 ? (float)hp / max : 0;
    }

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
        var gameConfig = Pool.Get<GameManager>();
        RefreshHp();

        tuviText.text =
            $"{progression.CurrentExp}/{gameConfig.inforLevels[progression.Level - 1].tuviRequireNextLevel}";
        Slider expSlider = tuviText.transform.parent.GetComponent<Slider>();
        expSlider.value = (float)progression.CurrentExp /
                          gameConfig.inforLevels[progression.Level - 1].tuviRequireNextLevel;

        iconLevel.sprite = listSpriteIconLevel[progression.Level - 1];
        iconLevel.SetNativeSize();
        nameText.text = charData.Name;
        characterGraphic.SetGender(charData.Gender);
        characterGraphic.SetSkin(charData.Appearance.Skins, charData.Appearance.Colors);
    }

    private void OnDestroy()
    {
        Observer.RemoveObserver(EventList.GamePlay.UPDATE_PLAYER_STATS, UpdatePlayerStats);
    }
}