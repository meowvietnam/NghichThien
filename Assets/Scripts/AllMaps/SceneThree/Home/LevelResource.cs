using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LuyenDan;
using MemoryPack;
using Network;
using Network.Extensions.MemoryPack;
using Network.Extensions;
using Sirenix.OdinInspector;

[Serializable]
[MemoryPackable]
public partial class InfoLevel
{
    public int tuviRequireNextLevel;
    public int hpMaxBuff;
    public int thanThucBuff;
    public int donTocBuff;
    public int thoNguyenBuff;
    public List<ItemRef> RequireItem;
    public List<ItemRef> BuffItem;
    [MemoryPackIgnore]
    public bool IsSpecialLevelUp => RequireItem.Count > 0;
}
[CreateAssetMenu(menuName = "LevelResource")]

public class LevelResource : ScriptableObject, IMemorySerializable<LevelResource>
{
    private static LevelResource instance;
    public static LevelResource Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<LevelResource>("Progression/LevelResource");
            }

            return instance;
        }
    }
    public InfoLevel[] inforLevels = {
        new() { tuviRequireNextLevel = 20000, hpMaxBuff = 15, thanThucBuff = 1, donTocBuff = 1, thoNguyenBuff = 0 },
        new() { tuviRequireNextLevel = 50000, hpMaxBuff = 15, thanThucBuff = 1, donTocBuff = 1, thoNguyenBuff = 0 },
        new() { tuviRequireNextLevel = 144000, hpMaxBuff = 80, thanThucBuff = 2, donTocBuff = 2, thoNguyenBuff = 100 },
        new() { tuviRequireNextLevel = 400000, hpMaxBuff = 40, thanThucBuff = 2, donTocBuff = 2, thoNguyenBuff = 0 },
        new() { tuviRequireNextLevel = 800000, hpMaxBuff = 40, thanThucBuff = 2, donTocBuff = 2, thoNguyenBuff = 0 },
        new() { tuviRequireNextLevel = 1200000, hpMaxBuff = 300, thanThucBuff = 8, donTocBuff = 8, thoNguyenBuff = 300 },
        new() { tuviRequireNextLevel = 1577000, hpMaxBuff = 150, thanThucBuff = 4, donTocBuff = 4, thoNguyenBuff = 0 },
        new() { tuviRequireNextLevel = 3211000, hpMaxBuff = 150, thanThucBuff = 4, donTocBuff = 4, thoNguyenBuff = 0 },
        new()
        {
            tuviRequireNextLevel = 6480000, hpMaxBuff = 800, thanThucBuff = 16, donTocBuff = 16, thoNguyenBuff = 500
        },
        new() { tuviRequireNextLevel = 19440000, hpMaxBuff = 400, thanThucBuff = 8, donTocBuff = 8, thoNguyenBuff = 0 },
        new() { tuviRequireNextLevel = 38880000, hpMaxBuff = 400, thanThucBuff = 8, donTocBuff = 8, thoNguyenBuff = 0 },
        new()
        {
            tuviRequireNextLevel = 77760000, hpMaxBuff = 8000, thanThucBuff = 32, donTocBuff = 32, thoNguyenBuff = 1000
        },
        new()
        {
            tuviRequireNextLevel = 94422000, hpMaxBuff = 4000, thanThucBuff = 16, donTocBuff = 16, thoNguyenBuff = 0
        },
        new()
        {
            tuviRequireNextLevel = 144411000, hpMaxBuff = 4000, thanThucBuff = 16, donTocBuff = 16, thoNguyenBuff = 0
        },
        new() { tuviRequireNextLevel = 200000000, hpMaxBuff = 0, thanThucBuff = 0, donTocBuff = 0, thoNguyenBuff = 0 }
    };

    public void Deserialize(IReader reader, ref LevelResource value)
    {
        value.inforLevels = reader.ReadArray<InfoLevel>();
    }

    public int ByteSize()
    {
        return inforLevels.ByteSize();
    }

    public void Serialize(IWriter writer)
    {
        writer.WriteArray(inforLevels);
    }

    [Button]
    public void Export()
    {
        var path = Path.Combine(Application.dataPath, "Exports", "level-resources.bin");
        var writer = new Writer(ByteSize());
        writer.Write(this);
        File.WriteAllBytes(path, writer.Buffer);
        Debug.Log(path);
    }
}
