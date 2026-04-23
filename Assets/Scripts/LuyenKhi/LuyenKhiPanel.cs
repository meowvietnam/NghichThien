using System.Collections.Generic;
using System.Linq;
using LuyenDan;
using Sirenix.OdinInspector;
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
        [field: SerializeField] public Bag Bag { get; private set; }
        [SerializeField] private Button changeTypeButton;
        [SerializeField] private Button backButton;
        private IBackContext backContext;
        [SerializeField] private List<ItemRef> items;

        private void Start()
        {
            SetBackContext(this);
            SetEventButton(changeTypeButton,OnChangeType);
            SetEventButton(backButton,OnBack);
            Bag.Init(items);
        }

        public void SetBackContext(IBackContext backContext)
        {
            this.backContext = backContext;
        }
        private void OnChangeType()
        {
            LuyenKhiPage.SetActive(false);
            SelectTypePage.SetActive(true);
            SetBackContext(SelectTypePage);
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

        [Button]
        public void RandomItem()
        {
            items = Resources.Materials.OrderBy(x => Random.value)
                .Select(x => new ItemRef()
                {
                    Count = Random.Range(1,10),
                    Id = x.Id,
                    Type = ItemType.Material
                })
                .Take(100)
                .ToList();
        }
    }
}
