using CreateCharacter;
using LongDHModules;
using Network;
using Network.Extensions;
using Network.Models;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
namespace Network.Core
{

    public class IAPHandler : PanelGroup, IClientHandler, IStoreListener
    {
        private IStoreController m_StoreController;
        private IExtensionProvider m_StoreExtensionProvider;
        // Khai báo các ID sản phẩm
        // CÁC GÓI TIÊN NGỌC ĐỒNG BỘ TỪ UI THỰC TẾ
        public const string PRODUCT_100_TIEN_NGOC = "100_tien_ngoc";       // Sơ Cấp (10.000 VND)
        public const string PRODUCT_550_TIEN_NGOC = "550_tien_ngoc";       // Tích Lũy (50.000 VND)
        public const string PRODUCT_1150_TIEN_NGOC = "1150_tien_ngoc";     // Phú Hộ (100.000 VND)
        public const string PRODUCT_2550_TIEN_NGOC = "2550_tien_ngoc";     // Hào Phú (200.000 VND)
        public const string PRODUCT_6550_TIEN_NGOC = "6550_tien_ngoc";     // Đại Gia (500.000 VND)
        public const string PRODUCT_13550_TIEN_NGOC = "13550_tien_ngoc";   // Thần Tài (1.000.000 VND)

        // GÓI QUÀ ĐẶC BIỆT (GIỮ LẠI THEO CODE CŨ CỦA BABE)
        public const string PRODUCT_START_PACK = "start_pack";
        [SerializeField] private Popup popupMain;
        [SerializeField]
        private Button btnStartPack;
        [SerializeField]
        private Button btn100TienNgoc;
        [SerializeField]
        private Button btn550TienNgoc;
        [SerializeField]
        private Button btn1150TienNgoc;
        [SerializeField]
        private Button btn2550TienNgoc;
        [SerializeField]
        private Button btn6550TienNgoc;
        [SerializeField]
        private Button btn13550TienNgoc;
        private static MessageTag[] handler =
        {
                      MessageTag.IAPResult
        };

        public MessageTag[] HandleTag => handler;
        public override void Init()
        {
            base.Init();
            InitIAP();
        }
        public void InitIAP()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            // Khai báo sản phẩm trên Store là Consumable để thả cửa cho mua nhiều lần
            builder.AddProduct(PRODUCT_100_TIEN_NGOC, ProductType.Consumable);
            builder.AddProduct(PRODUCT_550_TIEN_NGOC, ProductType.Consumable);
            builder.AddProduct(PRODUCT_1150_TIEN_NGOC, ProductType.Consumable);
            builder.AddProduct(PRODUCT_2550_TIEN_NGOC, ProductType.Consumable);
            builder.AddProduct(PRODUCT_6550_TIEN_NGOC, ProductType.Consumable);
            builder.AddProduct(PRODUCT_13550_TIEN_NGOC, ProductType.Consumable);
            builder.AddProduct(PRODUCT_START_PACK, ProductType.Consumable);
            UnityPurchasing.Initialize(this, builder);

            SetEventButton(btnStartPack, () =>
            {
                BuyProduct(PRODUCT_START_PACK);
            });
            SetEventButton(btn100TienNgoc, () =>
            {
                BuyProduct(PRODUCT_100_TIEN_NGOC);
            });
            SetEventButton(btn550TienNgoc, () =>
            {
                BuyProduct(PRODUCT_550_TIEN_NGOC);
            });
            SetEventButton(btn1150TienNgoc, () =>
            {
                BuyProduct(PRODUCT_1150_TIEN_NGOC);
            });
            SetEventButton(btn2550TienNgoc, () =>
            {
                BuyProduct(PRODUCT_2550_TIEN_NGOC);
            });
            SetEventButton(btn6550TienNgoc, () =>
            {
                BuyProduct(PRODUCT_6550_TIEN_NGOC);
            });
            SetEventButton(btn13550TienNgoc, () =>
            {
                BuyProduct(PRODUCT_13550_TIEN_NGOC);
            });
            if(Client.Instance.UserData.IsInfiniteEnergy)
            {
                btnStartPack.transform.parent.gameObject.SetActive(false);
            }    
            else
            {
                btnStartPack.transform.parent.gameObject.SetActive(true);

            }
        }// HÀM KÍCH HOẠT MUA ĐỒ (Gắn vào nút Bấm Mua trên UI)
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
        }// HÀM CHẠY KHI GOOGLE KHỞI TẠO IAP THÀNH CÔNG (Bắt buộc phải gán biến ở đây)
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            m_StoreController = controller;
            m_StoreExtensionProvider = extensions;
            Debug.Log("Unity IAP đã khởi tạo thành công hoàn toàn!");
        }

        // HÀM CHẠY KHI USER TRẢ TIỀN CHO GOOGLE THÀNH CÔNG
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            string receipt = args.purchasedProduct.receipt; // Chuỗi biên lai mã hóa cực kỳ quan trọng
            string productId = args.purchasedProduct.definition.id;
            var iapModel = new IAPModel
            {
                ProductId = productId,
                ReceiptId = receipt
            };
            NetworkManager.Instance.Invoke(() => {
                // Gửi gói tin lên Server kiểm tra
                Client.Instance.Send(MessageTag.IAP, iapModel);

            });


            // BƯỚC KHÓA HÒM 2: Trả về Pending. Đóng băng giao dịch, đố thằng hack local nào ép Client mở được hòm quà
            return PurchaseProcessingResult.Pending;
        }

        // HÀM ĐƯỢC GỌI KHI SERVER ĐÃ CHECK HÓA ĐƠN XỊN VÀ BÁO TRỞ LẠI CLIENT
        public void OnServerResponseIapSuccess(string productId)
        {
            if (m_StoreController != null)
            {
                Product product = m_StoreController.products.WithID(productId);
                if (product != null)
                {
                    // BƯỚC KHÓA HÒM 3: Lúc này mới xác nhận tiêu thụ với Google để đóng hóa đơn, tránh bị hoàn tiền
                    m_StoreController.ConfirmPendingPurchase(product);
                    Debug.Log($"[IAP] Hoàn tất tiêu thụ gói {productId} thành công!");
                }
            }
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"IAP Khởi tạo thất bại: {error}");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message = null)
        {
            Debug.LogError($"IAP Khởi tạo thất bại: {error} - Lỗi: {message}");
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
            if (tag == MessageTag.IAPResult)
            {
                var result = reader.Read<IAPResult>();
                if (result.Success)
                {
                    var charData = Client.Instance.CharacterData;
                    charData.MoneyVip += result.RewardSnapshot.MoneyVip;
                    charData.Stats.LinhThach += result.RewardSnapshot.Money;
                    var listItemRef = result.RewardSnapshot.ItemRefs;
                    foreach (var item in listItemRef)
                    {
                        charData.InventoryManager.Add(item);

                    }
                    OnServerResponseIapSuccess(result.ProductId);
                }
                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        Pool.Get<EventRewardPanel>().InitShowEventReward(result.RewardSnapshot,"Cảm ơn đạo hữu đã góp gạo nuôi Admin");
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
    }
}
    
