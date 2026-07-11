using AllMaps;
using LongDH.LongDHModules;
using Network;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ComboSkillResource.NPCData;
using LongDHModules;
using MemoryPack;
using Snapshot;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Base;

public class NPCManager : ActionMapManager
{
    [SerializeField] public NPCData[] npcDatas; 
    public override int TimeMonthRandom => 12;
    public override string fileSaveTime => "OldTimeRandomNPC";

    public override void Init()
    {
        base.Init();
        RandomAction();
    }
    public override void RandomAction()
    {
        var mapManager = Pool.Get<MapManager>();
        List<int> GetIdMapNodeWithMonPhai(EMonPhai monPhai)
        {
            var mapManager = Pool.Get<MapManager>();
            var MapWithMonPhai = mapManager.FindMap(GameManager.ConvertEMonPhaiToEMap(monPhai));

            List<int> idMapNode = new List<int>();
            if (MapWithMonPhai == null) return idMapNode;

            // Sử dụng HashSet để lưu các map đã check, tránh trường hợp map A nối với map B rồi map B lại nối ngược về map A
            HashSet<int> visitedMaps = new HashSet<int>();

            // Độ sâu ngẫu nhiên từ 1 đến 3 bước di chuyển xung quanh môn phái
            int maxDepth = UnityEngine.Random.Range(1, 4);

            // Truyền thêm tham số currentDepth để kiểm soát độ sâu chính xác của từng nhánh
            FindMapInMap(MapWithMonPhai, 1);

            void FindMapInMap(Map map, int currentDepth)
            {
                // Điều kiện dừng 1: Vượt quá độ sâu cho phép
                if (currentDepth > maxDepth) return;

                // Điều kiện dừng 2: Map này đã được duyệt qua rồi (Chống tràn Stack hoàn toàn)
                if (visitedMaps.Contains(map.Id)) return;

                // Đánh dấu map đã duyệt và thêm vào danh sách kết quả
                visitedMaps.Add(map.Id);
                idMapNode.Add(map.Id);

                // Duyệt các map liên kết xung quanh
                if (map.Nodes != null)
                {
                    foreach (var item in map.Nodes)
                    {
                        if (item != null && item.NextMap != null)
                        {
                            // Đi tiếp vào map con, tăng độ sâu lên +1 cho riêng nhánh đó
                            FindMapInMap(item.NextMap, currentDepth + 1);
                        }
                    }
                }
            }

            return idMapNode;
        }
        var kimPhai = GetIdMapNodeWithMonPhai(EMonPhai.KimHongKiemPhai);
        var mocPhai = GetIdMapNodeWithMonPhai(EMonPhai.TrucSonTong);
        var thuyPhai = GetIdMapNodeWithMonPhai(EMonPhai.TinhHaKiemPhai);
        var hoaPhai = GetIdMapNodeWithMonPhai(EMonPhai.LyHoaMon);
        var thoPhai = GetIdMapNodeWithMonPhai(EMonPhai.HoaTranGiao);
        var kiemPhai = GetIdMapNodeWithMonPhai(EMonPhai.BachDeLau);

        int bachDeLauIndex = 0;
        int kimHongKiemPhaiIndex = 0;

        foreach (var npc in npcDatas)
        {
            if (npc.charStats.monPhai == Base.EMonPhai.None)
            {
                int randomMapId = mapManager.maps[UnityEngine.Random.Range(0, mapManager.maps.Length)].Id;
                npc.charStats.idNode = randomMapId;
            }
            else
            {
                switch (npc.charStats.monPhai)
                {
                    case EMonPhai.None:
                        break;

                    case EMonPhai.BachDeLau:
                        // Distribute evenly across kiemPhai
                        npc.charStats.idNode = kiemPhai[bachDeLauIndex];
                        bachDeLauIndex = (bachDeLauIndex + 1) % kiemPhai.Count;
                        break;

                    case EMonPhai.KimHongKiemPhai:
                        // Distribute evenly across kimPhai
                        npc.charStats.idNode = kimPhai[kimHongKiemPhaiIndex];
                        kimHongKiemPhaiIndex = (kimHongKiemPhaiIndex + 1) % kimPhai.Count;
                        break;

                    case EMonPhai.TrucSonTong:
                        npc.charStats.idNode = mocPhai[UnityEngine.Random.Range(0, mocPhai.Count)];
                        break;

                    case EMonPhai.TinhHaKiemPhai:
                        npc.charStats.idNode = thuyPhai[UnityEngine.Random.Range(0, thuyPhai.Count)];
                        break;

                    case EMonPhai.LyHoaMon:
                        npc.charStats.idNode = hoaPhai[UnityEngine.Random.Range(0, hoaPhai.Count)];
                        break;

                    case EMonPhai.HoaTranGiao:
                        npc.charStats.idNode = thoPhai[UnityEngine.Random.Range(0, thoPhai.Count)];
                        break;

                    default:
                        break;
                }
            }
        }

        base.RandomAction();
    }
#if UNITY_EDITOR
    
    [Button]
    private void OnUpdateDataNPCData()
    {
        // Điền chính xác đường dẫn folder bạn muốn quét tại đây
        string[] searchFolders = new string[] { "Assets/Scripts/ComboSkillResource/NPCData" };

        // Truyền mảng searchFolders vào làm tham số thứ 2 của FindAssets
        string[] guids = AssetDatabase.FindAssets("t:NPCData", searchFolders);

        npcDatas = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<NPCData>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(asset => asset != null && !(asset is EnemyData))
            .ToArray();

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();

        Debug.Log($"Đã cập nhật {npcDatas.Length} NPC từ folder chỉ định.");
    }

    [Button]
    public void Export()
    {
        var npcDatas = this.npcDatas.Select(x => new NpcData(x)).ToArray();
        var bytes = MemoryPackSerializer.Serialize(npcDatas);
        var path = Path.Combine(Application.dataPath, "Exports", "npc-datas.bin");
        File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();
        Debug.Log(path);
    }
#endif
    
}
