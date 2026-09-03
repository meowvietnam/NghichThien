using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DP
{
    public class Shop : BaseBehaviour
    {
        [SerializeField] private ShopItem itemPrefab;
        [SerializeField] private RectTransform container;
        private Tween tween;
        private void Start()
        {
        }
        public Tween Show()
        {
            tween?.Kill();
            gameObject.SetActive(true);
            tween = transform.DOScale(Vector3.one,0.25f);
            return tween;
        }
        public void Hide()
        {
            tween?.Kill();
            tween = transform.DOScale(Vector3.zero,0.25f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
  
        public void Init(DongPhuShop shop)
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
            foreach (var seed in shop.Seeds)
            {
                var shopItem = Instantiate(itemPrefab, container);
                shopItem.Init(seed);
            }
        }
    }
}