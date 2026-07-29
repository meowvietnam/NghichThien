using System.Linq;
using Base;
using DG.Tweening;
using Network.Models;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Auction
{
    public class Player : CharacterGraphic
    {
        [SerializeField] private Transform selfTransform;
        [SerializeField] private GameObject selfObject;
        [SerializeField] private Text nameText;
        [SerializeField] private GameObject amountObject;
        [SerializeField] private Text amountText;
        [SerializeField] private Text statusText;
        [SerializeField] private Image statusImage;
        [SerializeField] private GameObject highlight;
        [SerializeField] private StatusResources statusResources;
        public PlayerAuctionModel Model { get; private set; }

        public void SetActive(bool active)
        {
            selfObject.SetActive(active);
        }

        public void SetData(PlayerAuctionModel model, bool activeHighlight = false)
        {
            Model = model;
            SetGender(model.Gender);
            var app = model.Appearance;
            SetSkin(app.Skins, app.Colors);
            SetName(model.Name);
            SetAmount(model.Amount);
            SetStatus(model.Status);
            Highlight(activeHighlight);
        }

        public bool IsMe()
        {
            return Model.CharId == AuctionPanel.Instance.Me.CharId;
        }

        public void SetName(string name)
        {
            nameText.text = name;
        }

        public void SetAmount(int amount)
        {
            Model.Amount = amount;
            var active = amount > 0;
            amountObject.SetActive(active);
            amountText.text = Utils.FormatCount(amount);
        }

        public void SetStatus(PlayerAuctionStatus status)
        {
            Model.Status = status;
            var data = statusResources.Datas.First(x => x.Status == status);
            statusImage.sprite = data.Icon;
            statusText.text = data.Name;
            statusText.color = data.Color;
        }

        public void SetSiblingIndex(int index)
        {
            selfTransform.SetSiblingIndex(index);
        }

        public void Highlight(bool active)
        {
            highlight.SetActive(active);
        }

        public bool IsActive()
        {
            return selfObject.activeInHierarchy;
        }

        public void ResetNewSession()
        {
            Model.ResetNewSession();
            SetStatus(Model.Status);
            SetAmount(Model.Amount);
        }
    }
}