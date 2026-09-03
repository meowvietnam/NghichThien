using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Events
{
    [Serializable]
    public class ItemEventData
    {
        [field: SerializeField] public ushort Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, PreviewField(50)] public Sprite Icon { get; private set; }
    }
    [CreateAssetMenu(menuName = "Events", fileName = "EventResources")]
    public class EventResources : ScriptableObject
    {
        [field: SerializeField] public ItemEventData[] Items { get; private set; }
        [field: SerializeField] public ButtonEvent[] EventButtons { get; private set; }
        [field: SerializeField] public ItemEventUI ItemEventUIPrefab { get; private set; }
        [field: SerializeField] public ItemEventMilestoneUI ItemEventMilestoneUI { get; private set; }
        private static EventResources instance;

        public static EventResources Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<EventResources>("Events/EventResources");
                }

                return instance;
            }
        }
    }
}
