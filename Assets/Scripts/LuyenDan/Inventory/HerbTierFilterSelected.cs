using System.Collections.Generic;
using Base;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan.Inventory
{
    public class HerbTierFilterSelected : MonoBehaviour
    {
        [SerializeField] private Text text;
        [SerializeField] private Transform direct;
        [SerializeField] private GameObject selectionList;
        [SerializeField] private Button btn;
        private bool isOpen;
        private string noFilter = "Toàn phẩm";

        private Dictionary<Tier, string> map = new()
        {
            { Tier.NhatPham, "Nhất phẩm" },
            { Tier.NhiPham, "Nhị phẩm" },
            { Tier.TamPham, "Tam phẩm" },
            { Tier.TuPham, "Tứ phẩm" },
            { Tier.NguPham, "Ngũ phẩm" },
            { Tier.LucPham, "Lục phẩm" }
        };

        private void Awake()
        {
            btn.onClick.AddListener(OnClick);
            NoFilter();
        }

        private void OnClick()
        {
            if (isOpen)
            {
                Colapse();
            }
            else
            {
                Open();
            }
        }

        public void NoFilter()
        {
            text.text = noFilter;
            Colapse();
        }

        public void Select(Tier tier)
        {
            text.text = map[tier];
            Colapse();
        }

        public void Open()
        {
            var rot = direct.localEulerAngles;
            rot.z = 180;
            direct.localEulerAngles = rot;
            isOpen = true;
            selectionList.SetActive(true);
        }

        public void Colapse()
        {
            var rot = direct.localEulerAngles;
            rot.z = 0;
            direct.localEulerAngles = rot;
            isOpen = false;
            selectionList.SetActive(false);
        }

        private void OnDestroy()
        {
            btn.onClick.RemoveAllListeners();
        }
    }
}