using AllMaps;
using LongDH.LongDHModules;
using Sirenix.OdinInspector;
using System.Linq;
using AllMaps.NgoDao;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

public class NgoDaoSlot : BaseBehaviour, INetworkEvent.ILearnNgoDao
{
    [SerializeField] public NgoDao ngoDaoSO;
    [SerializeField] private Button btn;
    [SerializeField] private Text nameText;
    [SerializeField] private Image icon;
    [SerializeField] private Text countRequire;
    [SerializeField] private Image blackImg;
    public SecureBool canLearn;
    public SecureBool isLearned;

    public void Init(NgoDao ngoDaoSO)
    {
        this.ngoDaoSO = ngoDaoSO;
        InitData();
        SetEventButton(btn, () => { Pool.Get<TabPanel>().ShowNgoDaoToolTip(this); });
    }

    private void InitData()
    {
        nameText.text = ngoDaoSO.name;
        icon.sprite = ngoDaoSO.spriteItem;
        icon.SetNativeSize();
        countRequire.text = ngoDaoSO.require.ToString();
    }

    public void LearnAndUpdateData()
    {
        Observer.Notify(EventList.Network.LEARN_NGODAO, this, ngoDaoSO.Id);
        //ngoDaoSO.Learn(this);
    }

    [Button]
    public void LearnedVisual()
    {
        blackImg.gameObject.SetActive(false);
        countRequire.transform.parent.gameObject.SetActive(false);
        icon.color = Color.white;
        nameText.transform.parent.gameObject.SetActive(false);
        isLearned = true;
        canLearn = false;
    }

    [Button]
    public void UnLearnVisual()
    {
        blackImg.gameObject.SetActive(true);
        countRequire.transform.parent.gameObject.SetActive(true);
        icon.color = Color.darkGray;
        nameText.transform.parent.gameObject.SetActive(true);
        isLearned = false;
    }

    [Button]
    public void GetComponent()
    {
        btn = GetComponent<Button>();
        nameText = transform.GetChild(2).transform.GetChild(0).GetComponent<Text>();
        icon = transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
        countRequire = transform.GetChild(3).transform.GetChild(0).GetComponent<Text>();
        blackImg = transform.GetChild(0).GetComponent<Image>();
    }

    public void SuccessLearnNgoDao(NgoDaoBuff buff, CreateCharacter.NgoDao ngoDao)
    {
        //Đã buff
        var TabPanel = Pool.Get<TabPanel>();
        TabPanel.SetNgoDaoDiem();
        LearnedVisual();
        Observer.Notify(EventList.GamePlay.UPDATE_PLAYER_STATS);

    }

    public void FailLearnNgoDao(string message)
    {
        Debug.Log(message);
    }
}