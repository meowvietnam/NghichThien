using Sirenix.OdinInspector;
using UnityEngine;

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
            var height = content.rect.height;
            var newHeight = Mathf.Clamp(height, minHeight, maxHeight);
            myTf.sizeDelta = new Vector2(myTf.sizeDelta.x, newHeight);
        }
    }
}