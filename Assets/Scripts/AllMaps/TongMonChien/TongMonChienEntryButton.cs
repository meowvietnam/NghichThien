using LongDH.LongDHModules;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps.TongMonChien
{
    /// <summary>Gắn vào nút ở AllMaps/giao diện tông môn để vào scene Tông môn chiến.</summary>
    [RequireComponent(typeof(Button))]
    public class TongMonChienEntryButton : MonoBehaviour
    {
        public const string SceneName = "TongMonChien";

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(Enter);
        }

        public static void Enter()
        {
            // GameManager.OnLoadScene gửi ChangeScene("TongMonChien"); server kiểm tra tông/thành rồi mới cho vào.
            Observer.Notify(EventList.GamePlay.LOAD_SCENE, SceneName);
        }
    }
}
