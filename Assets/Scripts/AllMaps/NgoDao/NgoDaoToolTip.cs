using AllMaps.NgoDao;
using UnityEngine;
using UnityEngine.UI;

public class NgoDaoToolTip : BaseBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Text nameText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text requireCountText;
    [SerializeField] private Text requireText;
    [SerializeField] private Button btnCamNgo;
    [SerializeField] private Button btnGreyCamNgo;
    [SerializeField] private Button btnMask;
    [SerializeField] private NgoDao ngoDaoSO;
    public void Init(NgoDaoSlot ngoDaoSlot , int levelNgoDaoRequire)
    {
        bool canLearn = ngoDaoSlot.canLearn;
        this.ngoDaoSO = ngoDaoSlot.ngoDaoSO;
        InitData(levelNgoDaoRequire);
        btnCamNgo.onClick.RemoveAllListeners();
        btnGreyCamNgo.onClick.RemoveAllListeners();
        btnMask.onClick.RemoveAllListeners();

        btnCamNgo.gameObject.SetActive(canLearn && !ngoDaoSlot.isLearned);
        btnGreyCamNgo.gameObject.SetActive(!btnCamNgo.gameObject.activeSelf);
        SetEventButton(btnMask, () =>
        {
            gameObject.SetActive(false);

        });
        if (canLearn && !ngoDaoSlot.isLearned)
        {
            SetEventButton(btnCamNgo, () =>
            {
                gameObject.SetActive(false);
                ngoDaoSlot.LearnAndUpdateData();

            });
        }
        else
        {
            SetEventButton(btnGreyCamNgo, () =>
            {
                gameObject.SetActive(false);

            });
        }
        RefreshTooltipLayout();


    }
    private void RefreshTooltipLayout()
    {
        var fitter = this.transform.GetComponentsInChildren<ContentSizeFitter>();
        if (fitter != null)
        {
            foreach (var item in fitter)
            {
                for (int i = 0; i < 3; i++)
                {
                    Canvas.ForceUpdateCanvases();
                    LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
                    item.enabled = false;
                    item.enabled = true;
                }
            }
        }
    }
    private void InitData(int levelNgoDaoRequire)
    {
        nameText.text = ngoDaoSO.name;
        icon.sprite = ngoDaoSO.spriteItem;
        icon.SetNativeSize();
        requireCountText.text =$"<color=#ffb143>【Yêu cầu điểm số】</color> {ngoDaoSO.require}";
        if(levelNgoDaoRequire > 0)
        {
            requireText.text = $"<color=#ffb143>【Điều kiện lĩnh ngộ】</color> {GetRequireText(ngoDaoSO.eTypeRequire)} chi đạo cảm ngộ đạt tới {NgoDaoResource.CamNgoText[levelNgoDaoRequire - 1]}";

        }
        descriptionText.text = ngoDaoSO.GetDes();
    }
    private string GetRequireText(BaseSkill.ETypeSkill[] requires)
    {
        string baseStr = "";
        for (int i = 0; i < requires.Length; i++)
        {
            baseStr += BaseSkill.GetTextTypeSkill(requires[i]);
            if(i<requires.Length)
            {
                baseStr += ", ";
            }    
        }
        return baseStr;
    }    
}
