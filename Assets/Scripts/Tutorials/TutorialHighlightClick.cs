using UnityEngine;
using UnityEngine.EventSystems;

namespace Tutorials
{
    public class TutorialHighlightClick : MonoBehaviour,
        IPointerClickHandler
    {
        [SerializeField] private TutorialHighlight tutorial;

        public void OnPointerClick(
            PointerEventData eventData)
        {
            tutorial.Hide();
        }
    }
}