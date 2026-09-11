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
            var seedUi = Instantiate(seedUIPrefab, container);
            seedUi.Init(seed.HerbId, seed.Count);
            seedUi.Clicked += OnClickSeed;

            void OnClickSeed()
            {
                context?.OnSelect(seedUi);
            }

            void OnCountUpdated(int count)
            {
                if (count > 0)
                {
                    seedUi.SetCount(count);
                }
                else
                {
                    seed.OnCountUpdated -= OnCountUpdated;
                    Destroy(seedUi.gameObject);
                }
            }

            seed.OnCountUpdated += OnCountUpdated;
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
            gameObject.SetActive(true);
            tween = transform.DOScale(Vector3.one, 0.25f);
            return tween;
        }

        public void Hide()
        {
            context?.OnClose();
            tween?.Kill();
            tween = transform.DOScale(Vector3.zero, 0.25f).OnComplete(() => { gameObject.SetActive(false); });
        }
    }
}