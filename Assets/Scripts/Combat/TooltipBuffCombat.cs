using CreateCharacter.CharStats;
using UnityEngine;
using UnityEngine.UI;
public class TooltipBuffCombat : Tooltip
{
    protected override void Awake()
    {

    }
    public override void SetText(string des)
    {
        base.SetText(des);
        FixVisual();
    }
    public override void SetText(string effective, string description)
    {
        base.SetText(effective, description);
        FixVisual();
    }
    public override void SetTextInfo(string infoName, string info)
    {
        base.SetTextInfo(infoName, info);
        FixVisual();
    }
    void FixVisual()
    {
        float signX = thisTransform.parent.transform.localScale.x;
        if (signX > 0)
        {
            thisTransform.pivot = new Vector2(0, 1f);
        }
        else
        {
            thisTransform.pivot = new Vector2(1, 1f);

        }
        Canvas.ForceUpdateCanvases();
        var contenSize = GetComponent<ContentSizeFitter>();
        for (int i = 0; i < 3; i++)
        {

            contenSize.enabled = false;
            contenSize.enabled = true;
        }
        thisTransform.localPosition = new Vector3(0, -45, 0);

    }
}
