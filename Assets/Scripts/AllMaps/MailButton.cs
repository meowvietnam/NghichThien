using System.Collections.Generic;
using System.Linq;
using Network;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps
{
    public class MailButton : BaseBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private GameObject redPoint;
        [SerializeField] private MailPanel mailPanel;

        private static MailButton instance;

        private void Start()
        {
            instance = this;
            SetEventButton(button, OnClick);
        }

        /// <summary>
        /// Tạo một chấm đỏ giống hệt chấm đỏ của nút Thư (nhân bản từ nó) ở góc trên phải của <paramref name="parent"/>,
        /// để các nút khác (Thành tựu...) báo "có thứ chờ nhận" cùng kiểu. Trả về null nếu chưa có nút Thư trong scene.
        /// </summary>
        public static GameObject CreateRedPoint(Transform parent)
        {
            if (instance == null) instance = FindFirstObjectByType<MailButton>(FindObjectsInactive.Include);
            if (instance == null || instance.redPoint == null || parent == null) return null;
            var go = Instantiate(instance.redPoint, parent);
            go.name = "RedPoint";
            var rect = (RectTransform)go.transform;
            var size = ((RectTransform)instance.redPoint.transform).sizeDelta;
            rect.anchorMin = rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = new Vector2(-size.x * 0.3f, -size.y * 0.3f);
            rect.localScale = Vector3.one;
            // Không tham gia LayoutGroup của nút (nếu có) và không chặn bấm.
            var layout = go.GetComponent<LayoutElement>();
            if (layout == null) layout = go.AddComponent<LayoutElement>();
            layout.ignoreLayout = true;
            foreach (var graphic in go.GetComponentsInChildren<Graphic>(true)) graphic.raycastTarget = false;
            return go;
        }

        private void OnClick()
        {
            mailPanel.SetActive(true);
        }

        private void OnDestroy()
        {
            if (instance == this) instance = null;
            button.onClick.RemoveAllListeners();
        }

        public void OnAdd(List<Mail> mails)
        {
            Refresh();
        }

        public void Refresh()
        {
            var mails = Client.Instance.CharacterData?.Social?.Mails ?? new List<Mail>();
            var isNeverRead = mails.Any(x => !x.IsRead);
            redPoint.SetActive(isNeverRead);
        }
    }
}