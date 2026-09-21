using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Boss
{
    public class BossMapMarker : MonoBehaviour
    {
        public long SpawnId { get; private set; }

        public static BossMapMarker Create(Transform node, BossStatus status)
        {
            var source = Resources.Load<CharacterVisualControl>("BossVisual");
            if (source == null)
            {
                Debug.LogError("Resources/BossVisual.prefab is missing or has no CharacterVisualControl.");
                return null;
            }

            var root = new GameObject("BossMapMarker", typeof(RectTransform), typeof(Canvas), typeof(BossMapMarker));
            var rect = root.GetComponent<RectTransform>();
            rect.SetParent(node, false);
            rect.localPosition = new Vector3(0f, 1.1f, 0f);
            rect.localScale = Vector3.one * 0.005f;
            rect.sizeDelta = new Vector2(340f, 400f);

            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.overrideSorting = true;
            canvas.sortingOrder = 31;

            var visual = Instantiate(source, rect);
            visual.name = "BossVisual";
            var visualRect = visual.GetComponent<RectTransform>();
            visualRect.anchorMin = Vector2.zero;
            visualRect.anchorMax = Vector2.one;
            visualRect.anchoredPosition = Vector2.zero;
            visualRect.sizeDelta = Vector2.zero;
            visualRect.localScale = Vector3.one;
            foreach (var graphic in visual.GetComponentsInChildren<Graphic>(true))
                graphic.raycastTarget = false;

            root.GetComponent<BossMapMarker>().Show(status, visual);
            return root.GetComponent<BossMapMarker>();
        }

        private void Show(BossStatus status, CharacterVisualControl visual)
        {
            SpawnId = status.SpawnId;
            visual.gameObject.SetActive(true);
            visual.Init(status.SkinId, CharacterStatsSecureBuilder.FromCharacterSnapshot(status.Boss));
        }
    }
}
