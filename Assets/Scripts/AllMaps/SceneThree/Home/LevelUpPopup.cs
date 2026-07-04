using Network;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPopup : Popup
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

    public void InitShow(int oldValueKhiHuyet,  int oldValueThoNguyen, int oldValueThanThuc, int oldValueDonToc)
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
        Show();
    }

}
