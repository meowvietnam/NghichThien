using System;
using Auction;
using Base;
using Network.Models.Clans;
using UnityEngine;
using UnityEngine.UI;

namespace Clan
{
    // Một dòng đơn xin gia nhập tông môn — hiển thị trong ClanPanel cho thành viên có quyền
    // AddMember duyệt (đồng ý / từ chối). Bản thân element không tự gọi mạng, chỉ báo sự kiện
    // ra ngoài để ClanPanel xử lý (giống cách ClanMemberElement báo sự kiện Clicked).
    public class ClanJoinRequestElement : BaseBehaviour
    {
        [SerializeField] private Text nameText;
        [SerializeField] private Text levelText;
        [SerializeField] private CharacterGraphic avatar;
        [SerializeField] private Button approveButton;
        [SerializeField] private Button rejectButton;

        public ClanJoinRequest Request { get; private set; }
        public int CharId => Request?.CharId ?? 0;

        public event Action<ClanJoinRequestElement> ApproveClicked;
        public event Action<ClanJoinRequestElement> RejectClicked;

        private void Awake()
        {
            SetEventButton(approveButton, () => ApproveClicked?.Invoke(this));
            SetEventButton(rejectButton, () => RejectClicked?.Invoke(this));
        }

        public void Init(ClanJoinRequest request)
        {
            Request = request;
            SetText(nameText, request != null && !string.IsNullOrWhiteSpace(request.Name) ? request.Name : "—");
            SetText(levelText, request != null
                ? (GameManager.levelWithText.TryGetValue(request.Level, out var text) ? text : $"Cảnh giới {Math.Max(1, request.Level)}")
                : string.Empty);
            SetAvatar(request);
            SetProcessing(false);
        }

        public void Clear()
        {
            Request = null;
            SetText(nameText, string.Empty);
            SetText(levelText, string.Empty);
            if (avatar != null) avatar.gameObject.SetActive(false);
            SetProcessing(false);
        }

        private void SetAvatar(ClanJoinRequest request)
        {
            if (avatar == null) return;
            if (request == null)
            {
                avatar.gameObject.SetActive(false);
                return;
            }

            avatar.gameObject.SetActive(true);
            avatar.SetGender(request.Gender);

            var appearance = request.Appearance;
            if (appearance?.Skins == null || appearance.Skins.Count == 0) return;
            avatar.SetSkin(appearance.Skins, appearance.Colors);
        }

        // Vô hiệu 2 nút khi đang chờ kết quả duyệt/từ chối để tránh bấm trùng.
        public void SetProcessing(bool value)
        {
            if (approveButton != null) approveButton.interactable = !value && Request != null;
            if (rejectButton != null) rejectButton.interactable = !value && Request != null;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        private static void SetText(Text target, string value)
        {
            if (target != null) target.text = value ?? string.Empty;
        }

        private void OnDestroy()
        {
            ApproveClicked = null;
            RejectClicked = null;
        }
    }
}
