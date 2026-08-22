using UnityEngine;
using UnityEngine.UI;

namespace Rank
{
    public class ItemRewardUI : BaseBehaviour
    {
        [SerializeField] private Text text;

        public void SetText(string text)
        {
            this.text.text = text;
        }
    }
}