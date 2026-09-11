using UnityEngine;
using UnityEngine.UI;

public class ItemUI : BaseItemUI
{
    [SerializeField] public int count = 0;
    [SerializeField] private Text countText;
    public override bool IsSetNativeIcon => false;

    public void AddCount(int count)
    {
        this.count += count;
        countText.text = this.count.ToString();
    }
}