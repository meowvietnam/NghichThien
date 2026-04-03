using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps
{
    public class ClampHeight : MonoBehaviour
    {
        [SerializeField] private RectTransform myTf;
        [SerializeField] private RectTransform content;
        [SerializeField] private float minHeight;
        [SerializeField] private float maxHeight;

        [Button]
        public void Clamp()
        {
            var layout = content.GetComponent<LayoutElement>();
            float height;
            if (layout != null && layout.preferredHeight > 0)
                height = layout.preferredHeight;
            else
                height = LayoutUtility.GetPreferredHeight(content);

            var newHeight = Mathf.Clamp(height, minHeight, maxHeight);

            myTf.sizeDelta = new Vector2(myTf.sizeDelta.x, newHeight);
        }
    }
}