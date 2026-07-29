using Base;
using UnityEngine;
using UnityEngine.UI;

namespace Auction
{
    public class HighestBidder : MonoBehaviour
    {
        [SerializeField] private GameObject selfObject;
        [SerializeField] private Text nameText;
        [SerializeField] private Text priceText;

        public void SetName(string name)
        {
            nameText.text = name;
        }

        public void SetPrice(int price)
        {
            priceText.text = Utils.FormatCount(price);
        }
        public void SetActive(bool active)
        {
            selfObject.SetActive(active);
        }
    }
}