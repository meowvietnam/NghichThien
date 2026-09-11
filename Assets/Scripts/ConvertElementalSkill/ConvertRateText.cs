using UnityEngine;
using UnityEngine.UI;

namespace ConvertElementalSkill
{
    public class ConvertRateText : BaseBehaviour
    {
        [SerializeField] private Text text;
        [SerializeField] private GameObject selfObject;

        public void SetText(string text)
        {
            this.text.text = text;
        }

        public void SetActive(bool active)
        {
            selfObject.SetActive(active);
        }
    }
}