using Network;
using UnityEngine;
using UnityEngine.UI;
using Network.Models;
using LongDH.LongDHModules;

public class LevelUpPopup : BaseBehaviour
{
    [SerializeField] private Text txtValueKhiHuyet;
    [SerializeField] private Text txtValueThoNguyen;
    [SerializeField] private Text txtValueThanThuc;
    [SerializeField] private Text txtValueDonToc;

    [SerializeField] private Text txtOldValueKhiHuyet;
    [SerializeField] private Text txtOldValueThoNguyen;
    [SerializeField] private Text txtOldValueThanThuc;
    [SerializeField] private Text txtOldValueDonToc;

    [SerializeField] private Text txtLevel;
    [SerializeField] private Popup mainPopup;
    private void Awake()
    {
        Observer.AddObserver(EventList.Network.LEVEL_UP, OnLevelUp);
    }
    public void OnLevelUp(object[] datas)
    {
        var snapshot = (CharacterSnapshot)datas[0];
        var oldHpMax = snapshot.SinhMenh;
        var oldThoNguyen = snapshot.ThoNguyen;
        var oldThanThuc = snapshot.ThanThuc;
        var oldDonToc = snapshot.TocDoBay;
        InitShow(oldHpMax, oldThoNguyen, oldThanThuc, oldDonToc);
    }
    private void InitShow(int oldValueKhiHuyet,  int oldValueThoNguyen, int oldValueThanThuc, int oldValueDonToc)
    {
        var stats = Client.Instance.CharacterData.Stats;
        // Set old values
        txtOldValueKhiHuyet.text = oldValueKhiHuyet.ToString();
        txtOldValueThoNguyen.text = oldValueThoNguyen.ToString();
        txtOldValueThanThuc.text = oldValueThanThuc.ToString();
        txtOldValueDonToc.text = oldValueDonToc.ToString();

        // Set new values
        txtValueKhiHuyet.text = stats.SinhMenh.ToString();
        txtValueThoNguyen.text = stats.ThoNguyen.ToString();
        txtValueThanThuc.text = stats.ThanThuc.ToString();
        txtValueDonToc.text = stats.TocDoBay.ToString();
        txtLevel.text = GameManager.levelWithText[Client.Instance.CharacterData.Progression.Level];
        mainPopup.Show();
    }

}
