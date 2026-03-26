using LongDH.LongDHModules;
using UnityEngine;
using UnityEngine.UI;

public class BackSceneBtn : BaseBehaviour
{
    [SerializeField] private Button btn;
    private void Awake()
    {
        btn = GetComponent<Button>();
        SetEventButton(btn, () =>
        {
            Observer.Notify(EventList.GamePlay.BACK_SCENE);

        });
    }
}
