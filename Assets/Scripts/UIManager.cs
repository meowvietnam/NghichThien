using UnityEngine;

public class UIManager : BaseBehaviour
{
    private void Start()
    {
        var listPanel = transform.GetComponentsInChildren<PanelGroup>(true);
        foreach (var item in listPanel)
        {
            item.Init();
        }    
    }
}
