using System;
using Base;
using CreateCharacter;
using Network;
using Network.Core;
using UnityEngine;
using UnityEngine.UI;
using ClanModel = Network.Models.Clans.Clan;

namespace Clan
{
    public class ClanElement : BaseBehaviour
    {
        [Header("Thông tin tông môn")]
        [SerializeField] private Button mainButton;
        [SerializeField] private Text clanText;
        [SerializeField] private Text levelText;
        [SerializeField] private Image clanImage;
        [SerializeField] private Text memberCountText;

        [Header("Xem chi tiết")]
        private BaseClanInfo detailPanel;
        [SerializeField] private GameObject loadingState;
        [SerializeField] private Sprite defaultClanImage;

        private ClanModel clanModel;
        private ClanModel clanViewModel;
        private int imageLoadVersion;
        private string loadedImageUrl = string.Empty;
        private string loadingImageUrl = string.Empty;
        private int viewRequestVersion;
        private bool isLoading;

        public ClanModel Data => clanModel;
        public ClanModel ViewData => clanViewModel;
        public bool IsLoading => isLoading;
        public event Action<ClanModel> ViewCompleted;

        // Cho phép nơi sinh ra element (vd. ClanListPanel) gán panel xem chi tiết dùng chung,
        // vì các element được Instantiate từ prefab nên không thể kéo-thả sẵn tham chiếu này.
        public void SetDetailPanel(BaseClanInfo panel) => detailPanel = panel;

        private void Awake()
        {
            if (defaultClanImage == null && clanImage != null) defaultClanImage = clanImage.sprite;
        }

        private void Start()
        {
            SetEventButton(mainButton, OnClick);
            SetLoading(false);
        }

        public void Init(ClanModel clanModel)
        {
            var changedClan = this.clanModel?.Id != clanModel?.Id;
            if (changedClan)
            {
                viewRequestVersion++;
                clanViewModel = null;
                isLoading = false;
            }
            this.clanModel = clanModel;
            Render();
            if (changedClan) SetLoading(false);
        }

        public void Clear()
        {
            clanModel = null;
            clanViewModel = null;
            viewRequestVersion++;
            loadedImageUrl = string.Empty;
            loadingImageUrl = string.Empty;
            imageLoadVersion++;
            SetText(clanText, string.Empty);
            SetText(levelText, string.Empty);
            SetText(memberCountText, string.Empty);
            ApplyImage(defaultClanImage);
            SetLoading(false);
        }

        private void OnClick()
        {
            if (!isLoading) ShowClanInfo();
        }

        public async void ShowClanInfo()
        {
            if (clanModel == null || isLoading) return;

            var requestedClanId = clanModel.Id;
            var requestVersion = viewRequestVersion;
            isLoading = true;
            SetLoading(true);
            try
            {
                var viewModel = clanViewModel;
                if (viewModel == null)
                {
                    var handler = Client.Instance?.CoreModule?.GetHandler<ClanHandler>();
                    if (handler == null)
                    {
                        Notify("Chưa kết nối máy chủ");
                        return;
                    }

                    viewModel = await handler.ViewClan(requestedClanId);
                }

                if (requestVersion != viewRequestVersion || clanModel?.Id != requestedClanId) return;
                clanViewModel = viewModel;

                if (clanViewModel == null)
                {
                    Notify("Không tìm thấy tông môn này");
                    return;
                }

                ViewCompleted?.Invoke(clanViewModel);
                if (detailPanel != null) detailPanel.ShowClan(clanViewModel);
                else Debug.LogWarning($"{nameof(ClanElement)}: chưa cấu hình detailPanel", this);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                Notify("Không thể tải thông tin tông môn");
            }
            finally
            {
                if (requestVersion == viewRequestVersion)
                {
                    isLoading = false;
                    SetLoading(false);
                }
            }
        }

        private void Render()
        {
            if (defaultClanImage == null && clanImage != null) defaultClanImage = clanImage.sprite;
            var hasData = clanModel != null;
            SetText(clanText, hasData ? clanModel.Name : string.Empty);
            SetText(levelText, hasData ? $"Cấp {Math.Max(1, clanModel.Level)}" : string.Empty);

            var loadedMembers = clanModel?.Members?.Count ?? 0;
            var memberCount = hasData ? Math.Max(clanModel.MemberCount, loadedMembers) : 0;
            var maxMembers = hasData ? Math.Max(0, clanModel.MaxMembers) : 0;
            SetText(memberCountText, hasData ? $"{memberCount}/{maxMembers}" : string.Empty);

            if (mainButton != null) mainButton.interactable = hasData && !isLoading;
            LoadImage(clanModel?.ImageUrl);
        }

        private void LoadImage(string imageUrl)
        {
            var url = imageUrl?.Trim();
            if (string.IsNullOrEmpty(url))
            {
                imageLoadVersion++;
                loadedImageUrl = string.Empty;
                loadingImageUrl = string.Empty;
                ApplyImage(defaultClanImage);
                return;
            }

            if (url == loadedImageUrl && clanImage != null && clanImage.sprite != null) return;
            if (ImageLoader.TryGetCached(url, out var sprite))
            {
                imageLoadVersion++;
                loadedImageUrl = url;
                loadingImageUrl = string.Empty;
                ApplyImage(sprite);
                return;
            }

            if (url == loadingImageUrl) return;
            loadingImageUrl = url;
            var version = ++imageLoadVersion;
            ApplyImage(defaultClanImage);
            LoadImageAsync(url, version);
        }

        private async void LoadImageAsync(string url, int version)
        {
            try
            {
                var sprite = await ImageLoader.GetSpriteFromUrl(url);
                if (this == null || version != imageLoadVersion || clanModel?.ImageUrl?.Trim() != url) return;
                loadingImageUrl = string.Empty;
                if (sprite != null) loadedImageUrl = url;
                ApplyImage(sprite != null ? sprite : defaultClanImage);
            }
            catch (Exception exception)
            {
                if (this == null || version != imageLoadVersion) return;
                loadingImageUrl = string.Empty;
                Debug.LogException(exception, this);
                ApplyImage(defaultClanImage);
            }
        }

        private void SetLoading(bool value)
        {
            isLoading = value;
            if (loadingState != null) loadingState.SetActive(value);
            if (mainButton != null) mainButton.interactable = clanModel != null && !value;
        }

        private void ApplyImage(Sprite sprite)
        {
            if (clanImage == null) return;
            clanImage.sprite = sprite;
            clanImage.enabled = sprite != null;
        }

        private static void SetText(Text target, string value)
        {
            if (target != null) target.text = value ?? string.Empty;
        }

        private static void Notify(string message)
        {
            if (!string.IsNullOrWhiteSpace(message)) NotifyGroup.Instance?.Show(message);
        }

        private void OnDestroy()
        {
            imageLoadVersion++;
            ViewCompleted = null;
        }
    }
}
