using Base;
using CreateCharacter;
using LongDH.LongDHModules;
using Network;
using Spine.Unity;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PrepareCombatPanel : Popup , IUISingleton
{
    [SerializeField] private CharacterStatsSecure charTarget => Pool.Get<GameManager>().enemyCombat.charStats;
    [SerializeField] private PopupInfor popupInfor;
    [SerializeField] private Button btnAttack;
    [SerializeField] private Button btnPrepare;
    [SerializeField] private Button btnXemXet;
    [SerializeField] private Text txtNameUser;
    [SerializeField] private Text txtTarget;
    [SerializeField] private CharacterGraphic graphicUser;
    [SerializeField] private CharacterVisualControl graphicTarget;


    public void Init()
    {
        Pool.Set(this);
        SetEventButton(btnXemXet, () =>
        {
            popupInfor.InitPopDoXet(charTarget);
            SetVisualHaveEnemyAndCharacter(popupInfor.charVisualControl);



        });
        SetEventButton(btnAttack, () =>
        {
            var gameManager =  Pool.Get<GameManager>();
            if (gameManager.enemyCombat == null) return;
            Pool.Get<UIManager>().panelGroup.SetActive(false);
            Pool.Get<UIManager>().commonMask.gameObject.SetActive(false);
            this.Hide();
            Pool.Get<UIManager>().loadScene.gameObject.SetActive(true);

            SceneManager.LoadScene("Combat");

        });
        SetEventButton(btnPrepare, () =>
        {
            Pool.Get<TabPanel>().gameObject.SetActive(true);


        });

    }
    public override void Show(float delay = 0.5F, Action actionComplete = null)
    {
        base.Show(delay, actionComplete);
        txtNameUser.text = Client.Instance.CharacterData.Name;
        txtTarget.text = charTarget.nameUser;
        graphicUser.SetGender(Client.Instance.CharacterData.Gender);
        graphicUser.SetSkin(Client.Instance.CharacterData.Appearance.Skins, Client.Instance.CharacterData.Appearance.Colors);
        SetVisualHaveEnemyAndCharacter(graphicTarget);
        

    }
    protected override void ActionBtnClose()
    {
        base.ActionBtnClose();
        Pool.Get<GameManager>().currentETypeCombat = ETypeCombat.None;

    }
    private void SetVisualHaveEnemyAndCharacter(CharacterVisualControl graphicTarget)
    {
        var GameManager = Pool.Get<GameManager>();
        if (GameManager.enemyCombat != null)
        {
            graphicTarget.Init(GameManager.enemyCombat.eTypeVisual,GameManager.enemyCombat.charStats);


        }

    }
   

}
