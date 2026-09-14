using LongDH.LongDHModules;
using UnityEngine;
using UnityEngine.UI;

public class LoadSceneBtn : BaseBehaviour
{
    [SerializeField] private Button btn;
    [SerializeField] private string nameScene;

    /// <summary>Tên scene nút này sẽ mở. Public để hand-tut tìm đúng nút cần khoanh sáng
    /// (VD nút vào Khách Sạn = LoadSceneBtn có nameScene "Home") mà không phải gán tay.</summary>
    public string NameScene => nameScene;

    private void Awake()
    {
        btn = GetComponent<Button>();
        SetEventButton(btn, () => { Observer.Notify(EventList.GamePlay.LOAD_SCENE, nameScene); });
    }
}