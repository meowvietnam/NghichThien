using System;
using Base;
using CreateCharacter;
using DG.Tweening;
using Network;
using UnityEngine;
using UnityEngine.UI;

namespace DP
{
    public class InventoryUI : BaseBehaviour
    {
        private IOpenInventoryContext context;
        [SerializeField] private DongPhuSeedUI seedUIPrefab;
        [SerializeField] private RectTransform container;
        [SerializeField] private Text moneyText;
        [SerializeField] private Text moneyVipText;
        [SerializeField] private CharacterGraphic avatar;
        private Tween tween;
        private void Start()
        {
        }
        public void Init(DongPhuInventory inventory)
        {
            var charData = Client.Instance.CharacterData;
            var appearance = charData.Appearance;
            avatar.SetGender(charData.Gender);
            avatar.SetSkin(appearance.Skins, appearance.Colors);
            var seeds = inventory.Seeds;
            foreach (var seed in seeds)
            {
                OnAddSeed(seed);
            }

            inventory.OnAddSeed += OnAddSeed;
        }
        private void OnAddSeed(Seed seed)
        {
            var seedUi =  Instantiate(seedUIPrefab, container);
            seedUi.Init(seed.HerbId, seed.Count);
            seed.OnCountUpdated += (count) =>
            {
                if (count > 0)
                {
                    seedUi.SetCount(count);
                }
                else
                {
                    Destroy(seedUi.gameObject);
                }
            };
        }
        public void UpdateMoney()
        {
            var charData = Client.Instance.CharacterData;
            var money = charData.Stats.LinhThach;
            var moneyVip = charData.MoneyVip;
            moneyText.text = Utils.FormatCount(money);
            moneyVipText.text = Utils.FormatCount(moneyVip);
        }
        public Tween Show(IOpenInventoryContext context)
        {
            this.context = context;
            UpdateMoney();
            tween?.Kill();
            tween = transform.DOScale(Vector3.one,0.5f);
            return tween;
        }
        public void Hide()
        {
            context?.OnClose(); 
            tween?.Kill();
            tween = transform.DOScale(Vector3.zero,0.5f);
        }
    }
}