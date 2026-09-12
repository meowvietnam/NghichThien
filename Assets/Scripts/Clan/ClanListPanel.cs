using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreateCharacter;
using Network.Core;
using UnityEngine;
using UnityEngine.UI;
using ClanModel = Network.Models.Clans.Clan;

namespace Clan
{
    public class ClanListPanel : ClanPopup
    {
        private const int SearchLimit = 20;

        // Dùng cho 2 việc: (1) làm panel xem chi tiết chung cho mọi ClanElement trong danh sách
        // (ClanPanel tự ẩn tính năng thành viên khi IsViewingExternalClan = true), và
        // (2) chuyển sang hiển thị khi người chơi vừa gia nhập/tạo tông môn (OnClanChanged).
        [Header("ClanPanel dùng chung (xem chi tiết + tông môn của mình)")]
        [SerializeField] private ClanPanel clanPanel;

        [Header("Tìm kiếm")]
        [SerializeField] private InputField searchInput;
        [SerializeField] private Button searchButton;

        [Header("Kết quả")]
        [SerializeField] private ClanElement elementPrefab;
        [SerializeField] private RectTransform listContainer;
        [SerializeField] private Text resultCountText;
        [SerializeField] private GameObject emptyState;
        [SerializeField] private GameObject loadingState;

        private readonly List<ClanElement> elements = new();
        private int searchVersion;
        private bool isSearching;

        protected override void Start()
        {
            base.Start();
            SetEventButton(searchButton, () => _ = Search());
            if (searchInput != null) searchInput.onEndEdit.AddListener(OnSearchSubmit);

            if (listContainer == null && elementPrefab != null)
                listContainer = elementPrefab.transform.parent as RectTransform;

            CacheReusableElements();
            if (elementPrefab != null && elementPrefab.gameObject.scene.IsValid())
                elementPrefab.gameObject.SetActive(false);

            SetLoading(false);
            SetEmpty(false);
        }

        protected override void OnShown()
        {
            base.OnShown();
            // Mở panel là tìm luôn với từ khoá rỗng để có sẵn một danh sách tông môn duyệt qua,
            // người chơi gõ tên rồi bấm tìm/Enter (bàn phím ảo mobile) để lọc lại.
            if (searchInput != null) searchInput.text = string.Empty;
            _ = Search();
        }

        protected override void BindClanEvents(ClanHandler handler)
        {
            handler.ClanChanged += OnClanChanged;
        }

        protected override void UnbindClanEvents(ClanHandler handler)
        {
            handler.ClanChanged -= OnClanChanged;
        }

        private void OnClanChanged(ClanModel clan)
        {
            if (clan == null) return;
            Hide(0f);
            if (clanPanel != null) clanPanel.Show();
        }

        private void OnSearchSubmit(string value)
        {
            _ = Search();
        }

        public async Task Search()
        {
            if (isSearching) return;
            var handler = Handler;
            if (handler == null)
            {
                Notify("Kết nối tông môn chưa sẵn sàng");
                return;
            }

            var keyword = searchInput != null ? searchInput.text.Trim() : string.Empty;
            var version = ++searchVersion;
            isSearching = true;
            SetSearching(true);
            SetLoading(true);
            SetEmpty(false);
            try
            {
                var result = await handler.Search(keyword, SearchLimit);
                if (version != searchVersion) return;

                if (!result.Success)
                {
                    Notify(string.IsNullOrWhiteSpace(result.Message) ? "Không thể tìm kiếm tông môn" : result.Message);
                    Render(Array.Empty<ClanModel>());
                    return;
                }

                Render(result.Clans ?? Array.Empty<ClanModel>());
            }
            catch (Exception exception)
            {
                if (version != searchVersion) return;
                Debug.LogException(exception, this);
                Notify("Không thể tìm kiếm tông môn, vui lòng thử lại");
                Render(Array.Empty<ClanModel>());
            }
            finally
            {
                if (version == searchVersion)
                {
                    isSearching = false;
                    SetSearching(false);
                    SetLoading(false);
                }
            }
        }

        private void Render(ClanModel[] clans)
        {
            for (var i = 0; i < clans.Length; i++)
            {
                var element = GetOrCreateElement(i);
                if (element == null) break;
                element.Init(clans[i]);
                element.gameObject.SetActive(true);
            }

            for (var i = clans.Length; i < elements.Count; i++)
            {
                elements[i].Clear();
                elements[i].gameObject.SetActive(false);
            }

            if (resultCountText != null)
                resultCountText.text = clans.Length > 0 ? $"Tìm thấy {clans.Length} tông môn" : string.Empty;

            SetEmpty(clans.Length == 0);
        }

        private ClanElement GetOrCreateElement(int index)
        {
            while (elements.Count <= index)
            {
                if (elementPrefab == null || listContainer == null)
                {
                    Debug.LogWarning($"{GetType().Name}: chưa cấu hình elementPrefab/listContainer", this);
                    return null;
                }

                var element = Instantiate(elementPrefab, listContainer);
                element.name = $"ClanElement_{elements.Count}";
                ConfigureElement(element);
                elements.Add(element);
            }

            return elements[index];
        }

        private void CacheReusableElements()
        {
            if (listContainer == null) return;
            foreach (var element in listContainer.GetComponentsInChildren<ClanElement>(true))
            {
                if (element == null || element == elementPrefab || elements.Contains(element)) continue;
                ConfigureElement(element);
                elements.Add(element);
            }
        }

        // Xem chi tiết tông môn trong danh sách này luôn dùng chung ClanPanel của ClanListPanel
        // (panel này tự nhận biết đang xem tông môn của người khác qua IsViewingExternalClan),
        // nên gán trực tiếp thay vì để trống chờ kéo-thả tay trên từng element.
        private void ConfigureElement(ClanElement element)
        {
            if (clanPanel == null)
                Debug.LogWarning($"{GetType().Name}: chưa gán clanPanel để xem chi tiết tông môn", this);
            element.SetDetailPanel(clanPanel);
            element.Clear();
            element.gameObject.SetActive(false);
        }

        private void SetSearching(bool value)
        {
            isSearching = value;
            if (searchButton != null) searchButton.interactable = !value;
        }

        private void SetLoading(bool value)
        {
            if (loadingState != null) loadingState.SetActive(value);
        }

        private void SetEmpty(bool value)
        {
            if (emptyState != null) emptyState.SetActive(value);
        }

        private static void Notify(string message)
        {
            if (!string.IsNullOrWhiteSpace(message)) NotifyGroup.Instance?.Show(message);
        }

        protected override void OnDestroy()
        {
            searchVersion++;
            if (searchInput != null) searchInput.onEndEdit.RemoveListener(OnSearchSubmit);
            base.OnDestroy();
        }
    }
}
