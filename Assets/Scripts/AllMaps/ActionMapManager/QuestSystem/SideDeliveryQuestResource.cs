using AllMaps;
using Base;
using CreateCharacter;
using DG.Tweening;
using LongDH.LongDHModules;
using Network;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(SideDeliveryQuestResource), menuName = "Action Map/Side Quest/" + nameof(SideDeliveryQuestResource))]

public class SideDeliveryQuestResource : SideQuestResource
{
    public override List<CharacterStatsSecure> listNPCInteractive => new List<CharacterStatsSecure> { Pool.Get<SideQuestManager>().characterProviderQuest , Pool.Get<SideQuestManager>().characterStatsTakeDeliver };

    public GameObject prefabIconMail;

    public List<string> GetStartDialogs()
    {
        return new List<string>
    {
        $"Đạo hữu dừng bước! Ta có vài món đồ cần chuyển gấp tới tay người bạn của mình.",
        "Vạn sự nhờ vào đạo hữu, xin hãy bảo quản món đồ này cẩn thận và trao tận tay y!"
    };
    }

    // Lời thoại lúc HOÀN THÀNH (Người nhận đồ cảm ơn và phát thưởng)
    public override List<string> CompleteSideQuest => new List<string>
    {
        "Đường xa vất vả, đây là chút tiên ngọc coi như lộ phí ta gửi tặng. Xin hãy nhận lấy!"
    };
    public override void ActiveEventMap(Map map)
    {
        base.ActiveEventMap(map);
        var charRandom = Pool.Get<SideQuestManager>().characterProviderQuest;
        var mapManager = Pool.Get<MapManager>();

        var validNPCs = npcDatas.Where(npc =>
        {
            if (npc == null || npc.charStats == null) return false;

            // Tìm map dựa trên idNode của NPC
            var mapOfNPC = mapManager.FindMap(npc.charStats.idNode);

            // Điều kiện: Tìm thấy map và map đó phải thoả mãn IsEmptyAction
            return mapOfNPC != null && mapOfNPC.IsEmptyAction;
        }).ToArray();
        var charRandomActive = GetNPCDataSideQuestWithTargetLevel(validNPCs, Client.Instance.CharacterData.Progression.Level);
        Pool.Get<SideQuestManager>().characterStatsTakeDeliver = charRandomActive.charStats;
        var mapActive = Pool.Get<MapManager>().FindMap(charRandomActive.charStats.idNode);
        Pool.Get<SideQuestManager>().mapTakeDeliver = mapActive;
        Pool.Get<MapManager>().SetDataOnMap(prefabIconMail,mapActive);
        Pool.Get<SideQuestManager>().InitShowQuestHaveChar(() =>
        {
            CameraFollow.Instance.target = mapActive.transform;
            DOVirtual.DelayedCall(2f, () =>
            {
                CameraFollow.Instance.target = Pool.Get<Player>().transform;

            });
        }, GetStartDialogs(), charRandom);
    }
  
}
