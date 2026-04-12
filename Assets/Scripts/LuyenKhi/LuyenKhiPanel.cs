using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenKhi
{
    public class LuyenKhiPanel : BaseBehaviour, IBackContext
    {
        private static  LuyenKhiPanel instance;

        public static LuyenKhiPanel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<LuyenKhiPanel>();
                } 
                return instance;
            }
        }
        [field: SerializeField] public LuyenKhiResources Resources { get; private set; }
        [field: SerializeField] public SelectTypePage SelectTypePage { get; private set; }
        [field: SerializeField] public LuyenKhiPage LuyenKhiPage { get; private set; }
        [SerializeField] private Button changeTypeButton;
        [SerializeField] private Button backButton;
        
        private IBackContext backContext;
        private void Start()
        {
            SetBackContext(this);
            SetEventButton(changeTypeButton,OnChangeType);
            SetEventButton(backButton,OnBack);
        }

        public void SetBackContext(IBackContext backContext)
        {
            this.backContext = backContext;
        }
        private void OnChangeType()
        {
            LuyenKhiPage.SetActive(false);
            SelectTypePage.SetActive(true);
        }
        private void OnBack()
        {
            backContext.Back();
        }
        public void SelectSubEquipment(SubEquipmentType type)
        {
            SelectTypePage.SetActive(false);
            LuyenKhiPage.SetActive(true);
            LuyenKhiPage.SelectSubEquipment(type);
        }

        public void Back()
        {
        }
    }
}
