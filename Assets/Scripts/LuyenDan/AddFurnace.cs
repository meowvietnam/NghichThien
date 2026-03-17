using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan
{
    public class AddFurnace : MonoBehaviour
    {
        [SerializeField] private Button btn;
        [SerializeField] private GameObject thisObject;
        private void Start()
        {
            btn.onClick.AddListener(OnClick);
        }
        private void OnClick()
        {
            LuyenDanPanel.Instance.FurnaceInventory.SetActive(true);
        }

        public void SetActive(bool active)
        {
            thisObject.SetActive(active);
        }
        private void OnDestroy()
        {
            btn.onClick.RemoveAllListeners();
        }
    }
}
