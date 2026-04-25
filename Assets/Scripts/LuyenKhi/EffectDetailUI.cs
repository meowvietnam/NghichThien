using UnityEngine;
using UnityEngine.UI;

namespace LuyenKhi
{
    public class EffectDetailUI : MonoBehaviour
    {
        [SerializeField] private Text text;

        public void SetText(string text)
        {
            this.text.text = text;
        }
    }
}