using Base;
using CreateCharacter;
using LongDH.LongDHModules;
using LongDHModules;
using Network.Models.Clans;
using UnityEngine;
using UnityEngine.UI;
using CharacterGraphic = Auction.CharacterGraphic;

namespace Clan
{
    /// <summary>
    /// Popup chi tiết thành viên: hiện khi ấn vào bất kỳ ClanMemberElement nào trong
    /// ClanPanel (kể cả bản thân). Luôn hiện thông tin thành viên; các nút tương tác
    /// (phong chức theo ClanPanel.GetAssignableRoles/CanAssignRole/AssignRole, đuổi
    /// theo ClanPanel.HasPermission(KickMember)/KickMember) chỉ hiện khi hợp lệ.
    /// </summary>
    public class ClanMemberActionPopup : BaseBehaviour
    {
        [SerializeField] private GameObject selfObject;
        [SerializeField] private Text nameText;
        [SerializeField] private Text roleText;
        [SerializeField] private Text levelText;
        [SerializeField] private Text contribText;
        [SerializeField] private Text joinAtText;
        [SerializeField] private CharacterGraphic avatar;
        [SerializeField] private Button closeButton;
        [SerializeField] private Transform roleButtonContainer;
        [SerializeField] private Button roleButtonTemplate;
        [SerializeField] private Button kickButton;

        private ClanPanel panel;
        private ClanMember target;

        private void Awake()
        {
            if (selfObject == null) selfObject = gameObject;
            SetEventButton(closeButton, Hide);
            if (roleButtonTemplate != null) roleButtonTemplate.gameObject.SetActive(false);
            selfObject.SetActive(false);
        }

        public void Show(ClanPanel ownerPanel, ClanMemberElement element)
        {
            panel = ownerPanel;
            target = element != null ? element.Member : null;
            if (panel == null || target == null)
            {
                Hide();
                return;
            }

            SetText(nameText, string.IsNullOrWhiteSpace(target.Name) ? "—" : target.Name);
            if (roleText != null)
            {
                var (roleLabel, roleColor) = ClanMemberElement.GetRole(target.Role);
                roleText.text = roleLabel;
                roleText.color = roleColor;
            }

            var levelName = GameManager.levelWithText.TryGetValue(target.Level, out var text)
                ? text
                : target.Level > 0 ? $"Cảnh giới {target.Level}" : "—";
            SetText(levelText, levelName);
            SetText(contribText, $"Đóng góp: {Utils.FormatCount(target.ContribPoints)}");
            SetText(joinAtText, $"Gia nhập: {target.JoinAt:dd/MM/yyyy}");
            SetAvatar(target);

            BuildRoleButtons();
            RefreshKickButton();

            if (selfObject != null) selfObject.SetActive(true);
        }

        private void SetAvatar(ClanMember member)
        {
            if (avatar == null) return;

            avatar.gameObject.SetActive(true);
            avatar.SetGender(member.Gender);

            var appearance = member.Appearance;
            if (appearance?.Skins == null || appearance.Skins.Count == 0) return;
            avatar.SetSkin(appearance.Skins, appearance.Colors);
        }

        public void Hide()
        {
            ClearRoleButtons();
            if (selfObject != null) selfObject.SetActive(false);
            panel = null;
            target = null;
        }

        private void BuildRoleButtons()
        {
            ClearRoleButtons();
            if (roleButtonTemplate == null || roleButtonContainer == null || panel == null || target == null) return;

            foreach (var role in panel.GetAssignableRoles())
            {
                if (!panel.CanAssignRole(target, role)) continue;

                var button = Instantiate(roleButtonTemplate, roleButtonContainer);
                button.gameObject.SetActive(true);

                var (roleLabel, _) = ClanMemberElement.GetRole(role);
                var label = button.GetComponentInChildren<Text>();
                if (label != null) label.text = $"Phong {roleLabel}";

                var capturedRole = role;
                SetEventButton(button, () => AssignRole(capturedRole));
            }
        }

        private void ClearRoleButtons()
        {
            if (roleButtonContainer == null) return;
            for (var i = roleButtonContainer.childCount - 1; i >= 0; i--)
            {
                var child = roleButtonContainer.GetChild(i);
                if (roleButtonTemplate != null && child == roleButtonTemplate.transform) continue;
                Destroy(child.gameObject);
            }
        }

        private void RefreshKickButton()
        {
            if (kickButton == null) return;

            var canKick = panel != null && target != null &&
                          panel.HasPermission(ClanPermission.KickMember) &&
                          panel.CurrentMember != null &&
                          target.CharId != panel.CurrentMember.CharId &&
                          target.Role < panel.CurrentRole;

            kickButton.gameObject.SetActive(canKick);
            if (canKick) SetEventButton(kickButton, ConfirmKick);
        }

        private async void AssignRole(ClanRole role)
        {
            if (panel == null || target == null) return;
            var targetMember = target;
            var result = await panel.AssignRole(targetMember, role);
            if (result.Success) Hide();
            else NotifyGroup.Instance?.Show(string.IsNullOrWhiteSpace(result.Message)
                ? "Không thể phong chức"
                : result.Message);
        }

        private void ConfirmKick()
        {
            if (panel == null || target == null) return;
            var charId = target.CharId;
            var name = string.IsNullOrWhiteSpace(target.Name) ? "thành viên này" : target.Name;
            Pool.Get<PopupConfirmAction>()?.ShowPopConfirmAction(
                $"Đuổi {name} khỏi tông môn?",
                () => DoKick(charId));
        }

        private async void DoKick(int charId)
        {
            if (panel == null) return;
            var result = await panel.KickMember(charId);
            if (result.Success) Hide();
            else NotifyGroup.Instance?.Show(string.IsNullOrWhiteSpace(result.Message)
                ? "Không thể đuổi thành viên"
                : result.Message);
        }

        private static void SetText(Text target, string value)
        {
            if (target != null) target.text = value ?? string.Empty;
        }
    }
}
