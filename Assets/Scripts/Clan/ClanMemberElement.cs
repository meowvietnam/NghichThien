using System;
using Auction;
using Base;
using Network.Models.Clans;
using UnityEngine;
using UnityEngine.UI;

namespace Clan
{
    public class ClanMemberElement : BaseBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Text nameText;
        [SerializeField] private Text levelText;
        [SerializeField] private Sprite[] spriteLevels;
        [SerializeField] private Image levelImage;
        [SerializeField] private Text roleText;
        [SerializeField] private Text contribText;
        [SerializeField] private CharacterGraphic avatar;
        [SerializeField] private GameObject avatarFallback;

        private ClanMember member;

        public ClanMember Member => member;
        public int CharId => member?.CharId ?? 0;

        public event Action<ClanMemberElement> Clicked;

        private void Awake()
        {
            if (button == null) button = GetComponent<Button>();
            SetEventButton(button, () => Clicked?.Invoke(this));
        }

        public void Init(ClanMember member)
        {
            if (member == null)
            {
                Clear();
                return;
            }

            this.member = member;
            SetText(nameText, string.IsNullOrWhiteSpace(member.Name) ? "—" : member.Name);
            SetLevel(member.Level);
            SetRole(member.Role);
            SetContribution(member.ContribPoints);
            SetAvatar(member);
            if (button != null) button.interactable = true;
        }

        public void SetContribution(long contribPoints)
        {
            if (member != null) member.ContribPoints = contribPoints;
            SetText(contribText, Utils.FormatCount(contribPoints));
        }

        public void SetRole(ClanRole role)
        {
            if (member != null) member.Role = role;
            if (roleText == null) return;

            (roleText.text, roleText.color) = GetRole(role);
        }

        public void SetLevel(int level)
        {
            if (member != null) member.Level = level;

            var levelName = GameManager.levelWithText.TryGetValue(level, out var text)
                ? text
                : level > 0 ? $"Cảnh giới {level}" : "—";
            SetText(levelText, levelName);

            if (levelImage == null) return;

            var spriteIndex = level - 1;
            var hasSprite = spriteLevels != null &&
                            spriteIndex >= 0 &&
                            spriteIndex < spriteLevels.Length &&
                            spriteLevels[spriteIndex] != null;
            levelImage.enabled = hasSprite;
            levelImage.sprite = hasSprite ? spriteLevels[spriteIndex] : null;
        }

        public static (string text, Color color) GetRole(ClanRole role)
        {
            switch (role)
            {
                case ClanRole.Member:
                    return ("Đệ Tử", new Color32(103, 96, 91, 255));
                case ClanRole.Elder:
                    return ("Trưởng Lão", new Color32(50, 112, 96, 255));
                case ClanRole.ViceLeader:
                    return ("Phó Tộc Chủ", new Color32(174, 115, 48, 255));
                case ClanRole.Leader:
                    return ("Tộc Chủ", new Color32(145, 58, 50, 255));
                default:
                    return ("Không xác định", Color.gray);
            }
        }

        public void Clear()
        {
            member = null;
            SetText(nameText, string.Empty);
            SetText(levelText, string.Empty);
            SetText(roleText, string.Empty);
            SetText(contribText, string.Empty);

            if (levelImage != null)
            {
                levelImage.sprite = null;
                levelImage.enabled = false;
            }

            SetAvatarVisible(false);
            if (button != null) button.interactable = false;
        }

        private void SetAvatar(ClanMember clanMember)
        {
            var appearance = clanMember.Appearance;
            if (avatar == null || appearance?.Skins == null || appearance.Skins.Count == 0)
            {
                SetAvatarVisible(false);
                return;
            }

            try
            {
                avatar.gameObject.SetActive(true);
                avatar.SetGender(clanMember.Gender);
                avatar.SetSkin(appearance.Skins, appearance.Colors);
                if (avatar.graphic != null)
                {
                    avatar.graphic.enabled = true;
                    avatar.graphic.SetAllDirty();
                }

                SetAvatarVisible(true);
            }
            catch (Exception exception)
            {
                SetAvatarVisible(false);
                Debug.LogWarning($"Không thể hiển thị avatar của thành viên {clanMember.CharId}: {exception.Message}", this);
            }
        }

        private void SetAvatarVisible(bool visible)
        {
            if (avatar != null) avatar.gameObject.SetActive(visible);
            if (avatarFallback != null) avatarFallback.SetActive(!visible);
        }

        private static void SetText(Text target, string value)
        {
            if (target != null) target.text = value ?? string.Empty;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
