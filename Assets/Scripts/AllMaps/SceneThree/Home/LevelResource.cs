using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuyenDan;
[CreateAssetMenu(menuName = "LevelResource")]

public class LevelResource : ScriptableObject
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
    [Serializable]
    public class InfoLevel
    {
        public int tuviRequireNextLevel;
        public int hpMaxBuff;
        public int thanThucBuff;
        public int donTocBuff;
        public int thoNguyenBuff;
        public List<ItemRef> RequireItem = new();
        public List<ItemRef> BuffItem = new();
        public bool IsSpecialLevelUp => RequireItem.Count > 0;

    }
    public InfoLevel[] inforLevels = new InfoLevel[]
    {
        new InfoLevel
            { tuviRequireNextLevel = 20000, hpMaxBuff = 15, thanThucBuff = 1, donTocBuff = 1, thoNguyenBuff = 0 },
        new InfoLevel
            { tuviRequireNextLevel = 50000, hpMaxBuff = 15, thanThucBuff = 1, donTocBuff = 1, thoNguyenBuff = 0 },
        new InfoLevel
            { tuviRequireNextLevel = 144000, hpMaxBuff = 80, thanThucBuff = 2, donTocBuff = 2, thoNguyenBuff = 100 },
        new InfoLevel
            { tuviRequireNextLevel = 400000, hpMaxBuff = 40, thanThucBuff = 2, donTocBuff = 2, thoNguyenBuff = 0 },
        new InfoLevel
            { tuviRequireNextLevel = 800000, hpMaxBuff = 40, thanThucBuff = 2, donTocBuff = 2, thoNguyenBuff = 0 },
        new InfoLevel
            { tuviRequireNextLevel = 1200000, hpMaxBuff = 300, thanThucBuff = 8, donTocBuff = 8, thoNguyenBuff = 300 },
        new InfoLevel
            { tuviRequireNextLevel = 1577000, hpMaxBuff = 150, thanThucBuff = 4, donTocBuff = 4, thoNguyenBuff = 0 },
        new InfoLevel
            { tuviRequireNextLevel = 3211000, hpMaxBuff = 150, thanThucBuff = 4, donTocBuff = 4, thoNguyenBuff = 0 },
        new InfoLevel
        {
            tuviRequireNextLevel = 6480000, hpMaxBuff = 800, thanThucBuff = 16, donTocBuff = 16, thoNguyenBuff = 500
        },
        new InfoLevel
            { tuviRequireNextLevel = 19440000, hpMaxBuff = 400, thanThucBuff = 8, donTocBuff = 8, thoNguyenBuff = 0 },
        new InfoLevel
            { tuviRequireNextLevel = 38880000, hpMaxBuff = 400, thanThucBuff = 8, donTocBuff = 8, thoNguyenBuff = 0 },
        new InfoLevel
        {
            tuviRequireNextLevel = 77760000, hpMaxBuff = 8000, thanThucBuff = 32, donTocBuff = 32, thoNguyenBuff = 1000
        },
        new InfoLevel
        {
            tuviRequireNextLevel = 94422000, hpMaxBuff = 4000, thanThucBuff = 16, donTocBuff = 16, thoNguyenBuff = 0
        },
        new InfoLevel
        {
            tuviRequireNextLevel = 144411000, hpMaxBuff = 4000, thanThucBuff = 16, donTocBuff = 16, thoNguyenBuff = 0
        },
        new InfoLevel
            { tuviRequireNextLevel = 200000000, hpMaxBuff = 0, thanThucBuff = 0, donTocBuff = 0, thoNguyenBuff = 0 }
    };

}
