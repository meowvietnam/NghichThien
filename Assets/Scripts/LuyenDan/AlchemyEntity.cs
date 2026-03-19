using System.Linq;
using LuyenDan.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LuyenDan
{
    /// <summary>
    /// Base class cho mọi ô slot trong màn hình luyện đan.
    /// Xử lý hiển thị item (icon, tên, chất lượng) và sự kiện hover/click.
    /// </summary>
    public class AlchemyEntity : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // ── Serialized Fields ────────────────────────────────────────────────
        [SerializeField] private Button clickButton;
        [SerializeField] private GameObject selfObject;

        [TabGroup("Null Item")]
        [SerializeField] private GameObject emptySlot;

        [TabGroup("Has Item")]
        [SerializeField] private GameObject filledSlot;
        [TabGroup("Has Item")]
        [SerializeField] private Image itemIcon;
        [TabGroup("Has Item")]
        [SerializeField] private Image qualityBackground;
        [TabGroup("Has Item")]
        [SerializeField] private Image qualityIconUp;
        [TabGroup("Has Item")]
        [SerializeField] private Text itemNameText;
        [TabGroup("Has Item")]
        [SerializeField] private GameObject hoverHighlight;

        // ── Properties ───────────────────────────────────────────────────────

        /// <summary>Item UI đang được chọn trong slot này. Null nếu slot trống.</summary>
        public InventoryItemUI Item { get; protected set; }

        // ── Unity Lifecycle ──────────────────────────────────────────────────

        private void OnValidate()
        {
            clickButton = GetComponent<Button>();
            selfObject   = gameObject;

            var tf = selfObject.transform;
            emptySlot  = tf.Find("Null").gameObject;
            filledSlot = tf.Find("HasItem").gameObject;

            itemIcon          = filledSlot.transform.Find("IconMask/Icon").GetComponent<Image>();
            qualityBackground = filledSlot.transform.Find("Quality").GetComponent<Image>();
            qualityIconUp     = qualityBackground.transform.Find("QualityUp").GetComponent<Image>();
            hoverHighlight    = tf.Find("Selected").gameObject;
            itemNameText      = filledSlot.transform.Find("NameMask/NameText").GetComponent<Text>();
        }

        private void Start()
        {
            clickButton.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            clickButton.onClick.RemoveAllListeners();
        }

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>Bật/tắt toàn bộ GameObject của slot.</summary>
        public void SetActive(bool active) => selfObject.SetActive(active);

        /// <summary>
        /// Gọi khi luyện đan hoàn thành. Override trong subclass để xử lý hậu kỳ
        /// (trừ nguyên liệu, giảm độ bền lò, v.v.).
        /// </summary>
        public virtual void OnLuyenDanCompleted() { }

        /// <summary>Chọn một item UI vào slot. Override trong subclass.</summary>
        [Button]
        public virtual void Select(InventoryItemUI item)
        {
            Item = item;
        }

        // ── Pointer Events ───────────────────────────────────────────────────

        public void OnPointerEnter(PointerEventData eventData) => hoverHighlight.SetActive(true);
        public void OnPointerExit(PointerEventData eventData)  => hoverHighlight.SetActive(false);

        // ── Protected Helpers ────────────────────────────────────────────────

        /// <summary>
        /// Cập nhật toàn bộ UI hiển thị theo data của item.
        /// Truyền null để xóa slot về trạng thái trống.
        /// </summary>
        protected void SetData(IInventoryItemData data)
        {
            bool hasItem = data != null;
            emptySlot.SetActive(!hasItem);
            filledSlot.SetActive(hasItem);

            if (!hasItem) return;

            itemIcon.sprite  = data.Icon;
            itemNameText.text = data.Name;
            ApplyQualityTheme(data.Tier);
        }

        protected virtual void OnClick()
        {
            LuyenDanPanel.Instance.Alchermy.SetCurrentSelected(this);
        }

        // ── Private Helpers ──────────────────────────────────────────────────

        private void ApplyQualityTheme(Tier tier)
        {
            var quality = LuyenDanPanel.Instance.Resources.Qualities.First(q => q.Tier == tier);
            qualityBackground.sprite = quality.Background;
            qualityIconUp.sprite     = quality.IconUp;
            itemNameText.color       = quality.Color;
        }
    }
}