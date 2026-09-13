using AllMaps;
using Network.Models.Clans;
using UnityEngine;
using UnityEngine.UI;
using ClanModel = Network.Models.Clans.Clan;

namespace Clan
{
    public class ClanStronghold : Map
    {
        [SerializeField] private Text clanNameText;
        [SerializeField] private GameObject enter;
        [SerializeField] private Button enterButton;

        private ClanModel clan;

        private void Awake()
        {
            MapType = MapType.Stronghold;
            SetEventButton(enterButton, OpenClan);
        }

        public void SetClan(ClanModel value)
        {
            clan = value;
            if (clanNameText == null) return;

            clanNameText.text = value?.Name ?? string.Empty;
            if (clanNameText.transform.parent != null)
                clanNameText.transform.parent.gameObject.SetActive(true);
        }

        public override void OnPlayerEnter()
        {
            base.OnPlayerEnter();
            if (clanNameText != null && clanNameText.transform.parent != null)
                clanNameText.transform.parent.gameObject.SetActive(true);
            if (enter != null) enter.SetActive(true);
        }

        public override void OnPlayerExit()
        {
            if (enter != null) enter.SetActive(false);
        }

        private void OpenClan()
        {
            if (clan == null) return;
            var entryController = FindFirstObjectByType<ClanEntryController>(FindObjectsInactive.Include);
            entryController?.Open();
        }
    }
}
