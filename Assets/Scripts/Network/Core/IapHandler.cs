using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CreateCharacter;
using DG.Tweening;
using IAP;
using LongDHModules;
using Network;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace Network.Core
{
    public class IapHandler : PanelGroup, IClientHandler, IStoreListener
    {
        private IStoreController m_StoreController;

        private IExtensionProvider m_StoreExtensionProvider;

        // Khai báo các ID sản phẩm
        // CÁC GÓI TIÊN NGỌC ĐỒNG BỘ TỪ UI THỰC TẾ
        // public const string PRODUCT_100_TIEN_NGOC = "100_tien_ngoc"; // Sơ Cấp (10.000 VND)
        // public const string PRODUCT_550_TIEN_NGOC = "550_tien_ngoc"; // Tích Lũy (50.000 VND)
        // public const string PRODUCT_1150_TIEN_NGOC = "1150_tien_ngoc"; // Phú Hộ (100.000 VND)
        // public const string PRODUCT_2550_TIEN_NGOC = "2550_tien_ngoc"; // Hào Phú (200.000 VND)
        // public const string PRODUCT_6550_TIEN_NGOC = "6550_tien_ngoc"; // Đại Gia (500.000 VND)
        // public const string PRODUCT_13550_TIEN_NGOC = "13550_tien_ngoc"; // Thần Tài (1.000.000 VND)

        public const string PRODUCT_START_PACK = "start_pack";
        [SerializeField] public Popup popupMain;
        [SerializeField] private StartPackUI startPackUIPrefab;
        [SerializeField] private MoneyVipPackUI moneyVipUIPrefab;
        [SerializeField] private Sprite[] moneyVipSprites;
        [SerializeField] private RectTransform iapProductContainer;
        [SerializeField] private RectTransform mainIapProductContainer;
        // [SerializeField] private Button btnStartPack;
        // [SerializeField] private Button btn100TienNgoc;
        // [SerializeField] private Button btn550TienNgoc;
        // [SerializeField] private Button btn1150TienNgoc;
        // [SerializeField] private Button btn2550TienNgoc;
        // [SerializeField] private Button btn6550TienNgoc;
        // [SerializeField] private Button btn13550TienNgoc;
        private List<OwnerIapProduct> ownerIapProducts;
        private static MessageTag[] handler =
        {
            MessageTag.PurchaseIapResult
        };

        public MessageTag[] HandleTag => handler;

        public override void Init()
        {
            base.Init();
            var coreModule = Client.Instance.CoreModule;
            coreModule.RemoveHandler<IapHandler>();
            coreModule.AddHandler(this);
            InitIAP();
        }

        private void OnEnable()
        {
            StartCoroutine(RefreshUI());
        }

        public async void InitIAP()
        {
            Debug.Log($"[IapHandler] GetProducts Request");
            var reader = await Client.Instance.RequestWait(MessageTag.GetProductsRequest);
            ownerIapProducts = reader.MemRead<List<OwnerIapProduct>>();
            NetworkManager.Instance.Invoke(() =>
            {
                var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
                foreach (var ownerIapProduct in ownerIapProducts)
                {
                    var productId = ownerIapProduct.Product.StoreProductId;
                    var productType = (ProductType)ownerIapProduct.Product.ProductType;
                    builder.AddProduct(productId, productType);
                }
                UnityPurchasing.Initialize(this, builder);
                foreach (var ownerIapProduct in ownerIapProducts)
                {
                    var uiContext = new IapUIContext()
                    {
                        IapHandler = this,
                        StartPackUIPrefab =  startPackUIPrefab,
                        MoneyVipPackUIPrefab =  moneyVipUIPrefab,
                        MoneyVipSprites = moneyVipSprites,
                        OwnerIapProduct = ownerIapProduct,
                        IapProductContainer = iapProductContainer
                    };
                    ownerIapProduct.Product.Reward.InitUI(uiContext);
                }
                popupMain.Show();
                var fitter = this.transform.GetComponentsInChildren<ContentSizeFitter>();
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
                DOVirtual.DelayedCall(0,() =>
                {
                    popupMain.Hide();
                });

            });
        }

        private IEnumerator RefreshUI()
        {
            yield return null;
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(mainIapProductContainer);
        }
        public void BuyProduct(string productId)
        {
            if (m_StoreController != null)
            {
                Product product = m_StoreController.products.WithID(productId);
                if (product != null && product.availableToPurchase)
                {
                    m_StoreController.InitiatePurchase(product);
                }
            }
        } 

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            m_StoreController = controller;
            m_StoreExtensionProvider = extensions;
            Debug.Log("Unity IapPurchase đã khởi tạo thành công hoàn toàn!");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            string receipt = args.purchasedProduct.receipt; 
            Debug.Log(receipt);
            var purchaseToken = ExtractGooglePurchaseToken(receipt);
            string productId = args.purchasedProduct.definition.id;
            var iapModel = new IapModel
            {
                Store = Store.GooglePlay,
                ProductId = productId,
                PurchaseToken = purchaseToken
            };
            Client.Instance.Send(MessageTag.IapPurchase, iapModel);
            return PurchaseProcessingResult.Pending;
        }

        public void OnServerResponseIapSuccess(string productId)
        {
            if (m_StoreController != null)
            {
                Product product = m_StoreController.products.WithID(productId);
                if (product != null)
                {
                    m_StoreController.ConfirmPendingPurchase(product);
                    Debug.Log($"[IapPurchase] Hoàn tất tiêu thụ gói {productId} thành công!");
                }
            }
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"IapPurchase Khởi tạo thất bại: {error}");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message = null)
        {
            Debug.LogError($"IapPurchase Khởi tạo thất bại: {error} - Lỗi: {message}");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.LogError($"Mua sản phẩm {product.definition.id} thất bại. Lý do: {failureReason}");
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
                if (result.Success)
                {
                    result.Reward.Execute(Client.Instance.CharacterData);
                    OnServerResponseIapSuccess(result.ProductId);
                }
                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        var uiContext = new IapUIContext()
                        {
                            IapHandler = this,
                        };
                        result.Reward.ShowUISuccess(uiContext);
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
        private string ExtractGooglePurchaseToken(string receiptJson)
        {
            try
            {
                var receipt = JsonConvert.DeserializeObject<UnityIAPReceipt>(receiptJson);
                if (receipt?.Payload == null) return string.Empty;

                var payload = JsonConvert.DeserializeObject<GooglePlayPayload>(receipt.Payload);
                if (payload?.Json == null) return string.Empty;

                var purchaseData = JsonConvert.DeserializeObject<GooglePurchaseData>(payload.Json);
                return purchaseData?.PurchaseToken ?? string.Empty;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[IAP] Parse purchaseToken thất bại: {ex.Message}");
                return string.Empty;
            }
        }

        private class UnityIAPReceipt
        {
            [JsonProperty("Store")]         public string Store         { get; set; }
            [JsonProperty("TransactionID")] public string TransactionID { get; set; }
            [JsonProperty("Payload")]       public string Payload       { get; set; }
        }

        private class GooglePlayPayload
        {
            [JsonProperty("json")]      public string Json      { get; set; }
            [JsonProperty("signature")] public string Signature { get; set; }
        }

        private class GooglePurchaseData
        {
            [JsonProperty("purchaseToken")] public string PurchaseToken { get; set; }
        }
    }
}