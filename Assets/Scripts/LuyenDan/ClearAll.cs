using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan
{
    public class ClearAll : MonoBehaviour
    {
        [SerializeField] private Button clearButton;

        private void Start()
        {
            clearButton.onClick.AddListener(ClearSlot);
        }

        private void ClearSlot()
        {
            LuyenDanPanel.Instance.Alchermy.ClearAllSlot();
        }

        private void OnDestroy()
        {
            clearButton.onClick.RemoveAllListeners();
        }
    }
}