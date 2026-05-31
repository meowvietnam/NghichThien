using AllMaps;
using LongDH.LongDHModules;
using LongDHModules;
using Network;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ChapterIDNodeAtMonPhaiUser), menuName = "Action Map/Main Quest/" + nameof(ChapterIDNodeAtMonPhaiUser))]
public class ChapterIDNodeAtMonPhaiUser : MainQuestResource
{
    public override int[] IdNode => new int[] { Pool.Get<MapManager>().FindMap(GameManager.ConvertEMonPhaiToEMap(Client.Instance.CharacterData.MonPhai)).Id};
}
