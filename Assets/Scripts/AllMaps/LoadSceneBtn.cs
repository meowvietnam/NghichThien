using LongDH.LongDHModules;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneBtn : BaseBehaviour
{
    [SerializeField] private Button btn;
    [SerializeField] private string nameScene;
    private void Awake()
    {
        btn = GetComponent<Button>();
        SetEventButton(btn, () =>
        {
            Observer.Notify(EventList.GamePlay.LOAD_SCENE, nameScene);

        });
    }
}
