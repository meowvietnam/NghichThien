using System.Linq;
using System.Threading.Tasks;
using Network;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using UnityEngine;

namespace Events
{
    public class EventManager : BaseBehaviour
    {
        [SerializeField] private UIManager uiManager;
        [SerializeField] private RectTransform panelContainer;
        private void Start()
        {
            _ = GetActiveEvents();
        }
        
        private async Task GetActiveEvents()
        {
            await Task.Delay(1000);
            var client = Client.Instance;
            var reader = await client.RequestWait(MessageTag.GetAllActiveEvents);
            var eventIds = reader.MemRead<ushort[]>();
            Debug.Log($"Event count: {eventIds.Length}");
            var eventPrefabs = EventResources.Instance.EventButtons;
            var prefabs = eventIds.Select(x => eventPrefabs.First(i => i.EventId == x)).Select(x => x).ToArray();
            foreach (var eventPrefab in prefabs)
            {
                var eventButton = Instantiate(eventPrefab, transform);
                eventButton.Init(panelContainer);
                uiManager.InsertIcon(eventButton.gameObject);
            }
        }
    }
}