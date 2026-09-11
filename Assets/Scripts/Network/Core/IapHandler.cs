using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreateCharacter;
using DG.Tweening;
using Firebase.Analytics;
using IAP;
using LuyenDan;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Network.Core
{
    public class IapHandler : PanelGroup, IClientHandler
    {
        [SerializeField] public Popup popupMain;
        [SerializeField] private StartPackUI startPackUIPrefab;
        [SerializeField] private MoneyVipPackUI moneyVipUIPrefab;
        [SerializeField] private Sprite[] moneyVipSprites;
        [SerializeField] private RectTransform iapProductContainer;
        [SerializeField] private RectTransform mainIapProductContainer;
        [SerializeField] private Button btnStartPackPrefab;
        [SerializeField] private RectTransform iconListContainer;
        [SerializeField] private Button showExchangePopup;
        [SerializeField] private SelectNum selectNum;
        private List<OwnerIapProduct> ownerIapProducts;
        [SerializeField] private IapService iapService;
        [SerializeField] private MonthlyPassPackUI monthlyPassUIPrefab;

        private static MessageTag[] handler =
        {
            MessageTag.PurchaseIapResult
        };

        public MessageTag[] HandleTag => handler;
        public Dictionary<string, List<GameObject>> IapObjects { get; private set; } = new();

        public override void Init()
        {
            base.Init();
            var coreModule = Client.Instance.CoreModule;
            coreModule.RemoveHandler<IapHandler>();
            coreModule.AddHandler(this);
            InitExchangeMoneyVip();
            _ = InitIAP();
        }

        private void InitExchangeMoneyVip()
        {
            SetEventButton(showExchangePopup, ShowExchangePopup);
            selectNum.confirmAction.RemoveListener(OnConfirmExchange);
            selectNum.confirmAction.AddListener(OnConfirmExchange);
        }

        private void SetExchangeText(int value)
        {
            selectNum.SetText(GetExchangeText(value));
        }

        private void OnConfirmExchange()
        {
            var amount = selectNum.GetValue();
            selectNum.Hide(0.5f);
            _ = ExchangeMoneyVip(amount);
        }

        private string GetExchangeText(int value)
        {
            return $"Quy đổi {value} Tiên ngọc - {value * 100} Linh thạch";
        }

        private void ShowExchangePopup()
        {
            var charData = Client.Instance.CharacterData;
            var min = Math.Min(1, charData.MoneyVip);
            var max = charData.MoneyVip;
            selectNum.SetMin(min);
            selectNum.SetMax(max);
            selectNum.SetValue(min);
            selectNum.ValueChanged -= SetExchangeText;
            selectNum.ValueChanged += SetExchangeText;
            selectNum.SetText(GetExchangeText(min));
            selectNum.Show(0.5f);
        }

        private void OnEnable()
        {
            StartCoroutine(RefreshUI());
        }

        public async Task InitIAP()
        {
            try
            {
                if (iapService == null)
                {
                    NotifyGroup.Instance.Show("IAP chưa được cấu hình");
                    return;
                }

                await iapService.InitializeIAP();
                iapService.PurchasePending -= OnPurchasePending;
                iapService.PurchasePending += OnPurchasePending;
                iapService.IapFailed -= OnIapFailed;
                iapService.IapFailed += OnIapFailed;
                var reader = await Client.Instance.RequestWait(MessageTag.GetProductsRequest, iapService.Store);
                ownerIapProducts = reader.MemRead<List<OwnerIapProduct>>();
                ownerIapProducts ??= new List<OwnerIapProduct>();
                NetworkManager.Instance.Invoke(() =>
                {
                    iapService.FetchProducts(ownerIapProducts);
                    iapService.FetchPurchases();
                    foreach (var ownerIapProduct in ownerIapProducts)
                    {
                        var uiContext = new IapUIContext()
                        {
                            ProductId = ownerIapProduct.Product.StoreProductId,
                            IapHandler = this,
                            StartPackUIPrefab = startPackUIPrefab,
                            MoneyVipPackUIPrefab = moneyVipUIPrefab,
                            MoneyVipSprites = moneyVipSprites,
                            OwnerIapProduct = ownerIapProduct,
                            IapProductContainer = iapProductContainer,
                            BtnStartPackPrefab = btnStartPackPrefab,
                            IconListContainer = iconListContainer,
                            MonthlyPassPackUIPrefab = monthlyPassUIPrefab,
                        };
                        ownerIapProduct.Product.Reward?.InitUI(uiContext);
                    }

                    popupMain.Show();
                    var fitter = transform.GetComponentsInChildren<ContentSizeFitter>();
                    if (fitter != null)
                    {
                        foreach (var item in fitter)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                Canvas.ForceUpdateCanvases();
                                LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
                                item.enabled = false;
                                item.enabled = true;
                            }
                        }
                    }

                    DOVirtual.DelayedCall(0, () => { popupMain.Hide(); });
                });
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                NetworkManager.Instance.Invoke(() =>
                {
                    NotifyGroup.Instance.Show($"Khởi tạo IAP thất bại: {ex.Message}");
                });
            }
        }

        private void OnPurchasePending(IapModel model)
        {
            Client.Instance.Send(MessageTag.IapPurchase, model);
        }

        public async Task<Result> CanBuyProduct(Store store, string productId)
        {
            var request = new CanBuyProductRequest()
            {
                Store = store,
                ProductId = productId
            };
            var reader = await Client.Instance.MemRequestWait(MessageTag.CanBuyProductRequest, request);
            return reader.MemRead<Result>();
        }

        private void OnIapFailed(string message)
        {
            NetworkManager.Instance.Invoke(() => { NotifyGroup.Instance.Show(message); });
        }

        public async Task ExchangeMoneyVip(int amount)
        {
            try
            {
                var client = Client.Instance;
                var reader = await client
                    .RequestWait(MessageTag.ExchangeMoneyVipRequest, amount, 20000);
                var result = reader.MemRead<ExchangeMoneyVipResult>();
                var charData = client.CharacterData;
                var addLinhThach = result.LinhThach - charData.Stats.LinhThach;
                if (result.Success)
                {
                    charData.MoneyVip = result.MoneyVip;
                    charData.Stats.LinhThach = result.LinhThach;
                }

                NetworkManager.Instance.Invoke(() =>
                {
                    var notify = NotifyGroup.Instance;
                    if (!result.Success)
                    {
                        notify.Show(result.Message);
                    }
                    else
                    {
                        notify.Show($"Quy đổi được {addLinhThach} Linh thạch");
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                NetworkManager.Instance.Invoke(() => { NotifyGroup.Instance.Show($"Quy đổi thất bại: {ex.Message}"); });
            }
        }

        private IEnumerator RefreshUI()
        {
            yield return null;
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(mainIapProductContainer);
        }

        public void BuyProduct(string productId)
        {
            iapService.BuyProduct(productId);
        }

        public void OnServerResponseIapSuccess(string productId)
        {
            iapService.ConfirmPurchase(productId);
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        public void OnEnter()
        {
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.PurchaseIapResult)
            {
                var result = reader.Read<IapResult>();
                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        var reward = result.Reward;
                        var charData = Client.Instance.CharacterData;
                        OnServerResponseIapSuccess(result.ProductId);
                        var uiContext = new IapUIContext()
                        {
                            IapHandler = this,
                            ProductId = result.ProductId,
                        };
                        reward?.ShowUISuccess(uiContext);
                        NotifyGroup.Instance.Show("Nạp thành công!");
                        FirebaseAnalytics.LogEvent("iap_purchase", "productId", result.ProductId);
                    }
                    else
                    {
                        NotifyGroup.Instance.Show(result.Message);
                    }
                });
            }
        }

        public void OnLeave()
        {
        }

        private void OnDestroy()
        {
            if (iapService == null)
            {
                return;
            }

            iapService.PurchasePending -= OnPurchasePending;
            iapService.IapFailed -= OnIapFailed;
        }
    }
}