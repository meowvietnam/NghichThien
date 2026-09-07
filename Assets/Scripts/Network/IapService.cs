using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IAP;
using Network.Models;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Network
{
    public class IapService : MonoBehaviour
    {
        [SerializeField] private string environmentName = "production";

#if UNITY_IPHONE
        [ShowInInspector, ReadOnly] private Store store => Store.AppStore;
#else
        [ShowInInspector,ReadOnly] private Store store => Store.GooglePlay;
#endif
        public Store Store => store;
        private StoreController storeController;
        private readonly Dictionary<string, PendingOrder> pendingOrdersByProductId = new();
        private Task initializeTask;
        private bool isInitializing;
        private bool isInitialized;

        public bool IsInitialized => isInitialized;

        public IReadOnlyList<Product> Products
        {
            get
            {
                if (storeController == null)
                {
                    return Array.Empty<Product>();
                }

                return storeController.GetProducts().ToList();
            }
        }

        public event Action<List<Product>> ProductsFetched;
        public event Action<Orders> PurchasesFetched;
        public event Action<IapModel> PurchasePending;
        public event Action<Order> PurchaseConfirmed;
        public event Action<string> IapFailed;

        public async Task InitializeIAP()
        {
            if (isInitialized)
            {
                return;
            }

            if (initializeTask != null)
            {
                await initializeTask;
                return;
            }

            initializeTask = InitializeIAPInternal();
            await initializeTask;
        }

        private async Task InitializeIAPInternal()
        {
            isInitializing = true;
            var option = new InitializationOptions();
            option.SetEnvironmentName(environmentName);

            try
            {
                if (UnityServices.State != ServicesInitializationState.Initialized)
                {
                    await UnityServices.InitializeAsync(option);
                }

                storeController = UnityIAPServices.StoreController();
                storeController.OnStoreConnected += OnStoreConnected;
                storeController.OnStoreDisconnected += OnStoreDisconnected;
                storeController.OnPurchasePending += OnPurchasePending;
                storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
                storeController.OnPurchaseFailed += OnPurchaseFailed;
                storeController.OnPurchaseDeferred += OnPurchaseDeferred;
                storeController.OnProductsFetched += OnProductsFetched;
                storeController.OnProductsFetchFailed += OnProductsFetchFailed;
                storeController.OnPurchasesFetched += OnPurchasesFetched;
                storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
                storeController.ProcessPendingOrdersOnPurchasesFetched(true);
                await storeController.Connect();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                IapFailed?.Invoke($"Khởi tạo IAP thất bại: {ex.Message}");
                isInitializing = false;
                initializeTask = null;
            }
        }

        public void FetchProducts(List<OwnerIapProduct> products)
        {
            if (!EnsureStoreReady())
            {
                return;
            }

            if (products == null)
            {
                IapFailed?.Invoke("Danh sách sản phẩm IAP không hợp lệ");
                return;
            }

            var productDefinitions = products
                .Where(x => x?.Product != null && x.Product.IsActive && !string.IsNullOrEmpty(x.Product.StoreProductId))
                .Select(x => new ProductDefinition(x.Product.StoreProductId, (ProductType)x.Product.ProductType))
                .ToList();

            if (productDefinitions.Count == 0)
            {
                IapFailed?.Invoke("Không có sản phẩm IAP hợp lệ để tải");
                return;
            }

            storeController.FetchProducts(productDefinitions);
        }

        public void FetchPurchases()
        {
            if (!EnsureStoreReady())
            {
                return;
            }

            storeController.FetchPurchases();
        }

        public void BuyProduct(string productId)
        {
            if (!EnsureStoreReady())
            {
                return;
            }

            var product = storeController.GetProducts().FirstOrDefault(x => x.definition.id == productId);
            if (product == null)
            {
                IapFailed?.Invoke($"Không tìm thấy sản phẩm {productId}");
                return;
            }

            if (!product.availableToPurchase)
            {
                IapFailed?.Invoke($"Sản phẩm {productId} chưa sẵn sàng để mua");
                return;
            }

            storeController.PurchaseProduct(product);
        }

        public void RestorePurchases(Action<bool, string> callback = null)
        {
            if (!EnsureStoreReady())
            {
                callback?.Invoke(false, "IAP chưa được khởi tạo");
                return;
            }

            storeController.RestoreTransactions(callback);
        }

        public bool ConfirmPurchase(string productId)
        {
            if (!EnsureStoreReady())
            {
                return false;
            }

            if (!pendingOrdersByProductId.TryGetValue(productId, out var order))
            {
                IapFailed?.Invoke($"Không tìm thấy giao dịch đang chờ xác nhận: {productId}");
                return false;
            }

            storeController.ConfirmPurchase(order);
            return true;
        }

        public bool ConfirmPurchase(IapModel iapModel)
        {
            if (iapModel == null)
            {
                IapFailed?.Invoke("Dữ liệu xác nhận IAP không hợp lệ");
                return false;
            }

            return ConfirmPurchase(iapModel.ProductId);
        }

        private void OnStoreConnected()
        {
            isInitialized = true;
            isInitializing = false;
            Debug.Log("Unity IAP đã kết nối store thành công");
        }

        private void OnStoreDisconnected(StoreConnectionFailureDescription description)
        {
            isInitialized = false;
            isInitializing = false;
            initializeTask = null;
            IapFailed?.Invoke($"Mất kết nối store IAP: {description.message}");
        }

        private void OnPurchaseConfirmed(Order order)
        {
            var productId = GetProductId(order);
            if (!string.IsNullOrEmpty(productId))
            {
                pendingOrdersByProductId.Remove(productId);
            }

            switch (order)
            {
                case ConfirmedOrder:
                    PurchaseConfirmed?.Invoke(order);
                    Debug.Log($"Xác nhận giao dịch IAP thành công: {productId}");
                    break;
                case FailedOrder failedOrder:
                    IapFailed?.Invoke($"Xác nhận giao dịch IAP thất bại: {productId} - {failedOrder.FailureReason}");
                    break;
                default:
                    IapFailed?.Invoke($"Không xác định trạng thái xác nhận IAP: {productId}");
                    break;
            }
        }

        private void OnPurchasePending(PendingOrder order)
        {
            var productId = GetProductId(order);
            if (string.IsNullOrEmpty(productId))
            {
                IapFailed?.Invoke("Không tìm thấy productId trong giao dịch IAP");
                return;
            }

            pendingOrdersByProductId[productId] = order;
            var iapModel = new IapModel
            {
                Store = store,
                ProductId = productId,
                ReceiptJson = order.Info.Receipt
            };
            PurchasePending?.Invoke(iapModel);
        }

        private void OnPurchaseFailed(FailedOrder order)
        {
            IapFailed?.Invoke($"Mua IAP thất bại: {GetProductId(order)} - {order.FailureReason}");
        }

        private void OnPurchaseDeferred(DeferredOrder order)
        {
            IapFailed?.Invoke($"Giao dịch IAP đang chờ xử lý: {GetProductId(order)}");
        }

        private void OnProductsFetched(List<Product> products)
        {
            ProductsFetched?.Invoke(products);
        }

        private void OnProductsFetchFailed(ProductFetchFailed failed)
        {
            IapFailed?.Invoke($"Tải danh sách sản phẩm IAP thất bại: {failed.FailureReason}");
        }

        private void OnPurchasesFetched(Orders orders)
        {
            PurchasesFetched?.Invoke(orders);
        }

        private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription failed)
        {
            IapFailed?.Invoke($"Tải lịch sử giao dịch IAP thất bại: {failed.message}");
        }

        private bool EnsureStoreReady()
        {
            if (storeController != null && isInitialized)
            {
                return true;
            }

            IapFailed?.Invoke("IAP chưa được khởi tạo");
            return false;
        }

        private static string GetProductId(Order order)
        {
            return order?.CartOrdered.Items().FirstOrDefault()?.Product.definition.id;
        }

        private static string ExtractPurchaseToken(string receiptJson)
        {
            if (string.IsNullOrEmpty(receiptJson))
            {
                return string.Empty;
            }

            try
            {
                var receipt = JsonConvert.DeserializeObject<UnityIAPReceipt>(receiptJson);
                if (string.IsNullOrEmpty(receipt?.Payload))
                {
                    return receiptJson;
                }

                var payload = JsonConvert.DeserializeObject<GooglePlayPayload>(receipt.Payload);
                if (string.IsNullOrEmpty(payload?.Json))
                {
                    return receipt.Payload;
                }

                var purchaseData = JsonConvert.DeserializeObject<GooglePurchaseData>(payload.Json);
                return purchaseData?.PurchaseToken ?? payload.Json;
            }
            catch (Exception ex)
            {
                return receiptJson;
            }
        }

        private void OnDestroy()
        {
            if (storeController == null)
            {
                return;
            }

            storeController.OnStoreConnected -= OnStoreConnected;
            storeController.OnStoreDisconnected -= OnStoreDisconnected;
            storeController.OnPurchasePending -= OnPurchasePending;
            storeController.OnPurchaseConfirmed -= OnPurchaseConfirmed;
            storeController.OnPurchaseFailed -= OnPurchaseFailed;
            storeController.OnPurchaseDeferred -= OnPurchaseDeferred;
            storeController.OnProductsFetched -= OnProductsFetched;
            storeController.OnProductsFetchFailed -= OnProductsFetchFailed;
            storeController.OnPurchasesFetched -= OnPurchasesFetched;
            storeController.OnPurchasesFetchFailed -= OnPurchasesFetchFailed;
        }

        private class UnityIAPReceipt
        {
            [JsonProperty("Store")] public string Store { get; set; }
            [JsonProperty("TransactionID")] public string TransactionID { get; set; }
            [JsonProperty("Payload")] public string Payload { get; set; }
        }

        private class GooglePlayPayload
        {
            [JsonProperty("json")] public string Json { get; set; }
            [JsonProperty("signature")] public string Signature { get; set; }
        }

        private class GooglePurchaseData
        {
            [JsonProperty("purchaseToken")] public string PurchaseToken { get; set; }
        }
    }
}