using CreateCharacter;
using MemoryPack;
using Network.Core;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace IAP
{
    public class IapUIContext
    {
        public string ProductId { get; init; }
        public IapHandler IapHandler { get; init; }
        public StartPackUI StartPackUIPrefab { get; init; }
        public MoneyVipPackUI MoneyVipPackUIPrefab { get; init; }
        public MonthlyPassPackUI MonthlyPassPackUIPrefab { get; init; }
        public Button BtnStartPackPrefab { get; init; }
        public OwnerIapProduct OwnerIapProduct { get; init; }
        public IapProduct IapProduct => OwnerIapProduct.Product;
        public Sprite[] MoneyVipSprites { get; init; }
        public RectTransform IapProductContainer { get; init; }
        public RectTransform IconListContainer { get; init; }
    }
    [MemoryPackable]
    [MemoryPackUnion(0, typeof(StartPack))]
    [MemoryPackUnion(1, typeof(MoneyVipPack))]
    [MemoryPackUnion(2, typeof(MonthlyPassPack))]
    [MemoryPackUnion(3, typeof(MonthlyPassVipPack))]
    public partial interface IIapReward
    {
        void ShowUISuccess(IapUIContext context);
        void InitUI(IapUIContext context);
    }
}