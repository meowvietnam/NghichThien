using System;
using Base;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenKhi
{
    public class TierToggle : MonoBehaviour
    {
        [field: SerializeField] public Tier Tier { get; private set; }
        [field: SerializeField] private Toggle toggle;

        public bool IsSelected()
        {
            return toggle.isOn;
        }
    }
}