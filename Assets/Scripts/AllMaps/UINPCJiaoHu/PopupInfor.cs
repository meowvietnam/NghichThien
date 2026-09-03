using CreateCharacter;
using LongDH.LongDHModules;
using Network;
using Skill;
using System.Collections.Generic;
using LongDHModules;
using UnityEngine;
using UnityEngine.UI;

public class PopupInfor : Popup
{
    [Header("--- Pop Do Xet ---")]
    [SerializeField] public CharacterGraphic charVisualPopXemXet;
    [SerializeField] public CharacterVisualControl charVisualControl;
    [SerializeField] private Text textThanThuc;
    [SerializeField] private Text textTocDoBay;
    [SerializeField] private Text textNgoTinh;
    [SerializeField] private Text textTuChat;
    [SerializeField] private Text textThoNguyen;
    [SerializeField] private Text textSinhMenh;
    [SerializeField] private Text textTuoiTac;
    [SerializeField] private Text textLevel;
    [SerializeField] private Text textNamePopXemXet;
    [SerializeField] private Text textHaoCam;
    [SerializeField] private List<ItemInventory> listThanThongItemSlot;
    [SerializeField] private List<ItemInventory> listCongPhapItemSlot;
    protected override void ActionBtnClose()
    {
        base.ActionBtnClose();
        Pool.Get<GameManager>().Cleanup();

    }
    public void InitPopDoXet(CharacterStatsSecure characterStatsTarget)
    {
        Show();
        textNamePopXemXet.text = characterStatsTarget.nameUser;
        charVisualPopXemXet.SetGender(characterStatsTarget.Gender);
        charVisualPopXemXet.SetSkin(characterStatsTarget.Skins, characterStatsTarget.Colors);
        textThanThuc.text = characterStatsTarget.thanThuc.Value.ToString();
        textTocDoBay.text = characterStatsTarget.tocDoBay.Value.ToString();
        textNgoTinh.text = characterStatsTarget.ngoTinh.Value.ToString();
        textTuChat.text = characterStatsTarget.tuChat.Value.ToString();
        textThoNguyen.text = characterStatsTarget.thoNguyen.Value.ToString();
        textTuoiTac.text = characterStatsTarget.tuoiTac.Value.ToString();
        textSinhMenh.text = characterStatsTarget.hpMax.Value.ToString();
        textLevel.text = GameManager.levelWithText[characterStatsTarget.level];
        textHaoCam.text = Client.Instance.CharacterData.Social.GetRelationshipWithNPC(characterStatsTarget.idChar).ToString();
        List<Skill.ThanThong> listThanThongData = new List<Skill.ThanThong>();
        var gameConfig = Pool.Get<GameManager>();
        var ui = Pool.Get<UIManager>();
        foreach (var item in characterStatsTarget.thanThongEquip)
        {
            listThanThongData.Add(gameConfig.GetThanThongWithId(item.Value));
        }
        for (int i = 0; i < listThanThongItemSlot.Count; i++)
        {
            var item = listThanThongItemSlot[i];
            var data = i < listThanThongData.Count ? listThanThongData[i] : null;

            ui.InitVisualItemSkill(item, data);
        }
        ui.InitVisualItemSkill(listCongPhapItemSlot[0], gameConfig.GetCongPhapWithId(characterStatsTarget.chuTu1Used.Value));
        ui.InitVisualItemSkill(listCongPhapItemSlot[1], gameConfig.GetCongPhapWithId(characterStatsTarget.chuTu2Used.Value));
        ui.InitVisualItemSkill(listCongPhapItemSlot[2], gameConfig.GetCongPhapWithId(characterStatsTarget.phuTu1Used.Value));
        ui.InitVisualItemSkill(listCongPhapItemSlot[3], gameConfig.GetCongPhapWithId(characterStatsTarget.phuTu2Used.Value));
        ui.InitVisualItemSkill(listCongPhapItemSlot[4], gameConfig.GetCongPhapWithId(characterStatsTarget.phuTu3Used.Value));
        ui.InitVisualItemSkill(listCongPhapItemSlot[5], gameConfig.GetCongPhapWithId(characterStatsTarget.phuTu4Used.Value));
    }
}
