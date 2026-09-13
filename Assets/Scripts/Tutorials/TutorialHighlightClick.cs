using UnityEngine;
using UnityEngine.EventSystems;

namespace Tutorials
{
    public class TutorialHighlightClick : MonoBehaviour,
        IPointerClickHandler
    {
        [SerializeField] private TutorialHighlight tutorial;

        /// <summary>Gán bằng code khi component được thêm lúc runtime (xem TutorialHighlight.Awake).</summary>
        public void Init(TutorialHighlight highlight)
        {
            tutorial = highlight;
        }

        public void OnPointerClick(
            PointerEventData eventData)
        {
            tutorial.Hide();
        }
    }
}