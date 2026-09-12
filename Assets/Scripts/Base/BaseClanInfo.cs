using System;
using System.Collections.Generic;
using System.Linq;
using Clan;
using Network.Core;
using Network.Models.Clans;
using UnityEngine;
using UnityEngine.UI;
using ClanModel = Network.Models.Clans.Clan;

namespace Base
{
    public class BaseClanInfo : ClanPopup
    {
        [Header("Thông tin tông môn")]
        [SerializeField] protected Image clanImage;
        [SerializeField] protected Sprite defaultClanImage;
        [SerializeField] protected Text memberCountText;

        [Header("Danh sách thành viên")]
        [SerializeField] protected ClanMemberElement[] leaderElements;
        [SerializeField] protected ClanMemberElement memberElementPrefab;
        [SerializeField] protected RectTransform memberContainer;
        protected readonly List<ClanMemberElement> members = new();
        protected ClanModel clan;
        private string currentImageUrl = string.Empty;
        private string loadedImageUrl = string.Empty;
        private string loadingImageUrl = string.Empty;
        private int imageLoadVersion;
        private ClanModel pendingClanToShow;
        private bool showPendingClan;
        protected bool isViewingExternalClan;

        public ClanModel Data => clan;
        public IReadOnlyList<ClanMemberElement> MemberElements => members;
        // True khi đang xem tông môn của người khác (không phải tông môn của chính mình) —
        // các panel con dùng cờ này để ẩn những tính năng chỉ dành cho thành viên (đóng góp,
        // rời tông môn, ...). Phong chức/đuổi thành viên đã tự ẩn qua HasPermission/CurrentMember.
        public bool IsViewingExternalClan => isViewingExternalClan;

        protected override void Awake()
        {
            base.Awake();
            leaderElements ??= Array.Empty<ClanMemberElement>();
            if (defaultClanImage == null && clanImage != null) defaultClanImage = clanImage.sprite;
            if (memberContainer == null && memberElementPrefab != null)
                memberContainer = memberElementPrefab.transform.parent as RectTransform;

            foreach (var element in leaderElements)
            {
                if (element == null) continue;
                element.Clicked -= OnMemberElementClicked;
                element.Clicked += OnMemberElementClicked;
            }

            CacheReusableElements();
            if (memberElementPrefab != null && memberElementPrefab.gameObject.scene.IsValid())
                memberElementPrefab.SetActive(false);
        }

        protected override void BindClanEvents(ClanHandler handler)
        {
            handler.ClanChanged += OnClanChanged;
            handler.MemberAdded += OnMemberAdded;
            handler.MemberRemoved += OnMemberRemoved;
            handler.ContributionChanged += OnContributionChanged;
            handler.MemberPointsChanged += OnMemberPointsChanged;
            handler.MemberRoleChanged += OnMemberRoleChanged;
        }

        protected override void UnbindClanEvents(ClanHandler handler)
        {
            handler.ClanChanged -= OnClanChanged;
            handler.MemberAdded -= OnMemberAdded;
            handler.MemberRemoved -= OnMemberRemoved;
            handler.ContributionChanged -= OnContributionChanged;
            handler.MemberPointsChanged -= OnMemberPointsChanged;
            handler.MemberRoleChanged -= OnMemberRoleChanged;
        }

        protected override void OnShown()
        {
            if (showPendingClan)
            {
                showPendingClan = false;
                var value = pendingClanToShow;
                pendingClanToShow = null;
                isViewingExternalClan = value != null && value.Id != Handler?.CurrentClan?.Id;
                Init(value);
                return;
            }

            isViewingExternalClan = false;
            Init(Handler?.CurrentClan);
        }

        public void ShowClan(ClanModel value, float time = 0.25f)
        {
            pendingClanToShow = value;
            showPendingClan = true;
            Show(time);
        }

        public virtual void Init(ClanModel clan)
        {
            this.clan = clan;
            RefreshClanSummary();
            if (clan?.Members == null)
            {
                ClearElements();
                return;
            }

            var leaderSlots = leaderElements.Where(element => element != null).ToArray();
            var leaderModels = clan.Members
                .Where(member => member != null)
                .Where(x => x.Role != ClanRole.Member)
                .OrderByDescending(x => x.Role)
                .ThenByDescending(x => x.ContribPoints)
                .ThenBy(x => x.JoinAt)
                .Take(leaderSlots.Length)
                .ToList();

            var leaderIds = leaderModels.Select(member => member.CharId).ToHashSet();
            var memberModels = clan.Members
                .Where(member => member != null && !leaderIds.Contains(member.CharId))
                .OrderByDescending(x => x.Role)
                .ThenByDescending(x => x.ContribPoints)
                .ThenBy(x => x.JoinAt)
                .ToList();

            RenderLeaders(leaderSlots, leaderModels);
            RenderMembers(memberModels);
        }

        public virtual void Refresh()
        {
            Init(Handler?.CurrentClan ?? clan);
        }

        public virtual void RefreshClanSummary()
        {
            if (memberCountText != null)
            {
                var loadedMemberCount = clan?.Members?.Count ?? 0;
                var currentMemberCount = clan == null ? 0 : Math.Max(clan.MemberCount, loadedMemberCount);
                var maxMemberCount = Math.Max(0, clan?.MaxMembers ?? 0);
                memberCountText.text = $"{currentMemberCount}/{maxMemberCount}";
            }

            SetClanImage(clan?.ImageUrl);
        }

        public bool TryGetMemberElement(int charId, out ClanMemberElement element)
        {
            element = leaderElements.FirstOrDefault(value => value != null && value.CharId == charId)
                      ?? members.FirstOrDefault(value => value != null && value.CharId == charId);
            return element != null;
        }

        protected virtual void OnClanChanged(ClanModel value)
        {
            if (isViewingExternalClan && value?.Id != clan?.Id) return;
            if (value == null)
            {
                clan = null;
                ClearElements();
                Hide();
                return;
            }

            Init(value);
        }

        protected virtual void OnMemberAdded(ClanMember member)
        {
            if (member != null && member.ClanId == clan?.Id) Refresh();
        }

        protected virtual void OnMemberRemoved(ClanRemoveMember removed)
        {
            if (removed != null && removed.ClanId == clan?.Id) Refresh();
        }

        protected virtual void OnContributionChanged(ClanContribution contribution)
        {
            if (contribution != null && contribution.ClanId == clan?.Id) Refresh();
        }

        protected virtual void OnMemberPointsChanged(ClanMemberPointsChanged changed)
        {
            if (changed != null && changed.ClanId == clan?.Id) Refresh();
        }

        protected virtual void OnMemberRoleChanged(ClanMemberRoleChanged changed)
        {
            if (changed != null && changed.ClanId == clan?.Id) Refresh();
        }

        protected virtual void OnMemberElementClicked(ClanMemberElement element)
        {
        }

        private void RenderLeaders(IReadOnlyList<ClanMemberElement> slots, IReadOnlyList<ClanMember> models)
        {
            for (var index = 0; index < slots.Count; index++)
            {
                var element = slots[index];
                var hasMember = index < models.Count;
                if (hasMember) element.Init(models[index]);
                else element.Clear();
                element.SetActive(hasMember);
            }
        }

        private void RenderMembers(IReadOnlyList<ClanMember> models)
        {
            for (var index = 0; index < models.Count; index++)
            {
                var element = GetOrCreateElement(index);
                if (element == null) break;
                element.Init(models[index]);
                element.SetActive(true);
                var firstMemberIndex = memberElementPrefab != null && memberElementPrefab.transform.parent == memberContainer
                    ? memberElementPrefab.transform.GetSiblingIndex() + 1
                    : 0;
                element.transform.SetSiblingIndex(Math.Min(firstMemberIndex + index, memberContainer.childCount - 1));
            }

            for (var index = models.Count; index < members.Count; index++)
            {
                var element = members[index];
                if (element == null) continue;
                element.Clear();
                element.SetActive(false);
            }
        }

        private ClanMemberElement GetOrCreateElement(int index)
        {
            while (members.Count <= index)
            {
                if (memberElementPrefab == null || memberContainer == null)
                {
                    Debug.LogWarning($"{GetType().Name}: chưa cấu hình memberElementPrefab/memberContainer", this);
                    return null;
                }

                var element = Instantiate(memberElementPrefab, memberContainer);
                element.name = $"ClanMemberElement_{members.Count}";
                element.Clear();
                element.Clicked -= OnMemberElementClicked;
                element.Clicked += OnMemberElementClicked;
                members.Add(element);
            }

            return members[index];
        }

        private void CacheReusableElements()
        {
            if (memberContainer == null) return;
            var leaderSet = new HashSet<ClanMemberElement>(leaderElements.Where(value => value != null));
            foreach (var element in memberContainer.GetComponentsInChildren<ClanMemberElement>(true))
            {
                if (element == null || element == memberElementPrefab || leaderSet.Contains(element) || members.Contains(element))
                    continue;

                element.Clear();
                element.SetActive(false);
                element.Clicked -= OnMemberElementClicked;
                element.Clicked += OnMemberElementClicked;
                members.Add(element);
            }
        }

        private void ClearElements()
        {
            foreach (var element in leaderElements)
            {
                if (element == null) continue;
                element.Clear();
                element.SetActive(false);
            }

            foreach (var element in members)
            {
                if (element == null) continue;
                element.Clear();
                element.SetActive(false);
            }
        }

        private void SetClanImage(string imageUrl)
        {
            var normalizedUrl = imageUrl?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(normalizedUrl))
            {
                currentImageUrl = string.Empty;
                loadedImageUrl = string.Empty;
                loadingImageUrl = string.Empty;
                imageLoadVersion++;
                ApplyClanImage(defaultClanImage);
                return;
            }

            currentImageUrl = normalizedUrl;
            if (normalizedUrl == loadedImageUrl && clanImage != null && clanImage.sprite != null) return;

            if (ImageLoader.TryGetCached(normalizedUrl, out var cachedSprite))
            {
                imageLoadVersion++;
                loadedImageUrl = normalizedUrl;
                loadingImageUrl = string.Empty;
                ApplyClanImage(cachedSprite);
                return;
            }

            if (normalizedUrl == loadingImageUrl) return;
            loadingImageUrl = normalizedUrl;
            var version = ++imageLoadVersion;
            ApplyClanImage(defaultClanImage);
            LoadClanImage(normalizedUrl, version);
        }

        private async void LoadClanImage(string imageUrl, int version)
        {
            try
            {
                var sprite = await ImageLoader.GetSpriteFromUrl(imageUrl);
                if (this == null || version != imageLoadVersion || currentImageUrl != imageUrl) return;
                loadingImageUrl = string.Empty;
                if (sprite != null) loadedImageUrl = imageUrl;
                ApplyClanImage(sprite != null ? sprite : defaultClanImage);
            }
            catch (Exception exception)
            {
                if (this == null || version != imageLoadVersion) return;
                loadingImageUrl = string.Empty;
                Debug.LogException(exception, this);
                ApplyClanImage(defaultClanImage);
            }
        }

        private void ApplyClanImage(Sprite sprite)
        {
            if (clanImage == null) return;
            clanImage.sprite = sprite;
            clanImage.enabled = sprite != null;
        }

        protected override void OnDestroy()
        {
            imageLoadVersion++;
            foreach (var element in leaderElements)
            {
                if (element != null) element.Clicked -= OnMemberElementClicked;
            }

            foreach (var element in members)
            {
                if (element != null) element.Clicked -= OnMemberElementClicked;
            }

            base.OnDestroy();
        }
    }
}
