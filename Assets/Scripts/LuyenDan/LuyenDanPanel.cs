using System;
using UnityEngine;

namespace LuyenDan
{
    public class LuyenDanPanel : MonoBehaviour
    {
        private static LuyenDanPanel instance;

        public static LuyenDanPanel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<LuyenDanPanel>();
                }
                return instance;
            }
        }
        [field: SerializeField] public LuyenDanResources Resources { get; private set; }
        [field: SerializeField] public Inventory.Inventory FurnaceInventory { get; private set; }

        private void Start()
        {
          //  FurnaceInventory.Init();
        }
    }
}
