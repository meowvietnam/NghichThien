using System;
using System.Collections.Generic;
using System.IO;
using AllMaps;
using CreateCharacter;
using LongDHModules;
using Network.Core;
using Network.Models.Clans;
using UnityEngine;
using UnityEngine.UI;
using ClanModel = Network.Models.Clans.Clan;

namespace Clan
{
    public class CreateClanPanel : ClanPopup
    {
        [SerializeField] private ClanPanel clanPanel;
        [Header("Thông tin tông môn")]
        [SerializeField] private InputField nameInput;
        [SerializeField] private InputField descriptionInput;
        [SerializeField] private InputField announcementInput;
        [SerializeField] private Text selectedNodeText;
        [SerializeField] private Button selectNodeButton;
        [SerializeField] private Button createByMoneyVipButton;
        [SerializeField] private Button createByVndButton;

        [Header("Ảnh tông môn")]
        [SerializeField] private Image previewImage;
        [SerializeField] private Button pickImageButton;
        [SerializeField] private Text imageStatusText;

        [Header("Giá tạo tông môn (lấy từ ClanConfig)")]
        [SerializeField] private Text moneyVipPriceText;
        [SerializeField] private Text vndPriceText;

        [Header("Chọn vị trí trên AllMaps")]
        [Tooltip("Các UI cần ẩn khi chọn node. Để trống sẽ dùng UIManager.panelGroup.")]
        [SerializeField] private GameObject[] uiRootsToHide;

        private const int MaxImageDimension = 512;

        private readonly List<HiddenUiState> hiddenUiStates = new List<HiddenUiState>();
        private MapManager selectingMapManager;
        private int selectedNodeId = -1;
        private string imageUrl;
        private bool isCreating;
        private bool isUploadingImage;
        private Sprite previewSprite;

        public int SelectedNodeId => selectedNodeId;

        private readonly struct HiddenUiState
        {
            public readonly GameObject Target;
            public readonly bool WasActive;

            public HiddenUiState(GameObject target)
            {
                Target = target;
                WasActive = target != null && target.activeSelf;
            }
        }

        protected override void Start()
        {
            base.Start();
            SetEventButton(selectNodeButton, BeginSelectNode);
            SetEventButton(createByMoneyVipButton, CreateWithMoneyVip);
            SetEventButton(createByVndButton, CreateWithVnd);
            SetEventButton(pickImageButton, PickImage);

            if (nameInput != null) nameInput.characterLimit = CreateClanRequest.MaxNameLength;
            RefreshSelectedNodeText();
            RefreshImageStatusText();
            RefreshPriceTexts();
        }

        protected override void OnShown()
        {
            base.OnShown();
            RefreshPriceTexts();
        }

        private void RefreshPriceTexts()
        {
            var config = Handler?.CurrentConfig;
            if (moneyVipPriceText != null)
            {
                moneyVipPriceText.text = config != null
                    ? $"{config.CreateClanMoneyVipPrice:n0} Tiên Ngọc"
                    : "--";
            }

            if (vndPriceText != null)
            {
                vndPriceText.text = config != null
                    ? $"{config.CreateClanVndPrice:n0} VNĐ"
                    : "--";
            }
        }

        public void SetImageUrl(string value)
        {
            imageUrl = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
            RefreshImageStatusText();
        }

        public void PickImage()
        {
            if (isUploadingImage) return;
            NativeFilePicker.PickFile(OnImagePicked, "image/*");
        }

        private async void OnImagePicked(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            byte[] imageData;
            try
            {
                imageData = LoadAndEncodeImage(path);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                ShowMessage("Không đọc được ảnh đã chọn");
                return;
            }

            if (imageData == null || imageData.Length == 0)
            {
                ShowMessage("Ảnh không hợp lệ, vui lòng chọn ảnh khác");
                return;
            }

            if (!InitializeHandler())
            {
                ShowMessage("Kết nối tông môn chưa sẵn sàng");
                return;
            }

            SetUploadingImage(true);
            try
            {
                var result = await Handler.UploadImage(imageData);
                if (!result.Success)
                {
                    ShowMessage(result.Message);
                    return;
                }

                SetImageUrl(result.Url);
                ApplyPreviewTexture(imageData);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                ShowMessage("Không thể tải ảnh lên, vui lòng thử lại");
            }
            finally
            {
                SetUploadingImage(false);
            }
        }

        private static byte[] LoadAndEncodeImage(string path)
        {
            var rawData = File.ReadAllBytes(path);
            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            try
            {
                if (!ImageConversion.LoadImage(texture, rawData, false)) return null;

                if (texture.width > MaxImageDimension || texture.height > MaxImageDimension)
                {
                    var resized = ResizeTexture(texture, MaxImageDimension);
                    Destroy(texture);
                    texture = resized;
                }

                return texture.EncodeToJPG(85);
            }
            finally
            {
                Destroy(texture);
            }
        }

        private static Texture2D ResizeTexture(Texture2D source, int maxDimension)
        {
            var scale = (float)maxDimension / Mathf.Max(source.width, source.height);
            var targetWidth = Mathf.Max(1, Mathf.RoundToInt(source.width * scale));
            var targetHeight = Mathf.Max(1, Mathf.RoundToInt(source.height * scale));

            var renderTexture = RenderTexture.GetTemporary(targetWidth, targetHeight, 0, RenderTextureFormat.ARGB32);
            var previous = RenderTexture.active;
            Graphics.Blit(source, renderTexture);
            RenderTexture.active = renderTexture;

            var resized = new Texture2D(targetWidth, targetHeight, TextureFormat.RGBA32, false);
            resized.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
            resized.Apply();

            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTexture);
            return resized;
        }

        private void ApplyPreviewTexture(byte[] imageData)
        {
            if (previewImage == null) return;

            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!ImageConversion.LoadImage(texture, imageData, false))
            {
                UnityEngine.Object.Destroy(texture);
                return;
            }

            if (previewSprite != null)
            {
                if (previewSprite.texture != null) UnityEngine.Object.Destroy(previewSprite.texture);
                UnityEngine.Object.Destroy(previewSprite);
            }

            previewSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            previewImage.sprite = previewSprite;
            previewImage.preserveAspect = true;
        }

        private void SetUploadingImage(bool value)
        {
            isUploadingImage = value;
            if (pickImageButton != null) pickImageButton.interactable = !value;
            RefreshImageStatusText();
        }

        private void RefreshImageStatusText()
        {
            if (imageStatusText == null) return;
            imageStatusText.text = isUploadingImage
                ? "Đang tải ảnh lên..."
                : string.IsNullOrEmpty(imageUrl) ? "Chưa chọn ảnh" : "Đã chọn ảnh";
        }

        public void BeginSelectNode()
        {
            if (!UnityEngine.SceneManagement.SceneManager.GetSceneByName("AllMaps").isLoaded)
            {
                ShowMessage("Chỉ có thể chọn vị trí tông môn tại bản đồ AllMaps");
                return;
            }

            var mapManager = Pool.Get<MapManager>();
            if (mapManager == null)
            {
                ShowMessage("Bản đồ chưa sẵn sàng, vui lòng thử lại");
                return;
            }

            if (!mapManager.BeginNodeSelection(this, OnNodeSelected, OnNodeRejected, RestoreUi))
            {
                ShowMessage("Bản đồ đang được dùng để chọn một vị trí khác");
                return;
            }

            selectingMapManager = mapManager;
            HideUiForNodeSelection();
        }

        public void CancelSelectNode()
        {
            if (selectingMapManager != null && selectingMapManager.IsSelectingNode)
                selectingMapManager.CancelNodeSelection(this);
            else
                RestoreUi();
        }

        public void CreateWithMoneyVip()
        {
            CreateClan(CreateClanBy.MoneyVip);
        }

        public void CreateWithVnd()
        {
            CreateClan(CreateClanBy.Vnd);
        }

        private async void CreateClan(CreateClanBy createBy)
        {
            if (isCreating) return;
            if (!TryBuildRequest(createBy, out var request, out var validationMessage))
            {
                ShowMessage(validationMessage);
                return;
            }

            if (!InitializeHandler())
            {
                ShowMessage("Kết nối tông môn chưa sẵn sàng");
                return;
            }

            SetCreating(true);
            try
            {
                var result = await Handler.CreateClan(request);
                if (!result.Success)
                {
                    ShowMessage(result.Message);
                    return;
                }

                if (createBy == CreateClanBy.Vnd && !string.IsNullOrWhiteSpace(result.Limk))
                    Application.OpenURL(result.Limk);
                else if (!string.IsNullOrWhiteSpace(result.Message))
                    ShowMessage(result.Message);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                ShowMessage("Không thể tạo tông môn, vui lòng thử lại");
            }
            finally
            {
                SetCreating(false);
            }
        }

        private bool TryBuildRequest(CreateClanBy createBy, out CreateClanRequest request, out string message)
        {
            request = null;
            var clanName = nameInput == null ? string.Empty : nameInput.text.Trim();
            if (string.IsNullOrWhiteSpace(clanName))
            {
                message = "Tên tông môn không được để trống";
                return false;
            }

            if (clanName.Length > CreateClanRequest.MaxNameLength)
            {
                message = $"Tên tông môn tối đa {CreateClanRequest.MaxNameLength} ký tự";
                return false;
            }

            if (selectedNodeId < 0)
            {
                message = "Hãy chọn vị trí tông môn trên bản đồ";
                return false;
            }

            request = new CreateClanRequest
            {
                Name = clanName,
                Description = descriptionInput == null ? string.Empty : descriptionInput.text.Trim(),
                Announcement = announcementInput == null ? string.Empty : announcementInput.text.Trim(),
                NodeId = selectedNodeId,
                ImageUrl = imageUrl,
                CreateBy = createBy
            };
            message = null;
            return true;
        }

        private void OnNodeSelected(Map map)
        {
            selectedNodeId = map.Id;
            selectingMapManager = null;
            RestoreUi();
            RefreshSelectedNodeText();
        }

        private void OnNodeRejected(Map map)
        {
            selectingMapManager?.CancelNodeSelection(this);
            ShowMessage("Không thể đặt tông môn tại thành trì, hãy chọn một node thường");
        }

        private void HideUiForNodeSelection()
        {
            hiddenUiStates.Clear();
            if (uiRootsToHide != null && uiRootsToHide.Length > 0)
            {
                foreach (var target in uiRootsToHide) HideUiRoot(target);
                return;
            }

            var uiManager = Pool.Get<UIManager>();
            if (uiManager != null && uiManager.panelGroup != null)
                HideUiRoot(uiManager.panelGroup);
            else
                HideUiRoot(selfObject);
        }

        private void HideUiRoot(GameObject target)
        {
            if (target == null || hiddenUiStates.Exists(x => x.Target == target)) return;
            hiddenUiStates.Add(new HiddenUiState(target));
            target.SetActive(false);
        }

        private void RestoreUi()
        {
            for (var i = hiddenUiStates.Count - 1; i >= 0; i--)
            {
                var state = hiddenUiStates[i];
                if (state.Target != null) state.Target.SetActive(state.WasActive);
            }

            hiddenUiStates.Clear();
            selectingMapManager = null;
        }

        private void RefreshSelectedNodeText()
        {
            if (selectedNodeText != null)
                selectedNodeText.text = selectedNodeId < 0 ? "Chưa chọn vị trí" : $"Node {selectedNodeId}";
        }

        private void SetCreating(bool value)
        {
            isCreating = value;
            if (createByMoneyVipButton != null) createByMoneyVipButton.interactable = !value;
            if (createByVndButton != null) createByVndButton.interactable = !value;
        }

        private static void ShowMessage(string message)
        {
            if (!string.IsNullOrWhiteSpace(message)) NotifyGroup.Instance?.Show(message);
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

        protected override void OnDestroy()
        {
            CancelSelectNode();
            if (previewSprite != null)
            {
                if (previewSprite.texture != null) UnityEngine.Object.Destroy(previewSprite.texture);
                UnityEngine.Object.Destroy(previewSprite);
                previewSprite = null;
            }
            base.OnDestroy();
        }
    }
}
