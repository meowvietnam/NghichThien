using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LuyenKhi;
using MemoryPack;
using Network;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Clan
{
    [MemoryPackable]
    [Serializable]
    public partial class ScoreDivisorRule
    {
        [field: SerializeField] public int ScoreThreshold { get; private set; }
        [field: SerializeField]  public int Divisor { get; private set; }
    }
    [CreateAssetMenu(fileName = "ClanBuffResources")]
    public class ClanBuffResources : ScriptableObject, IMemorySerializable<ClanBuffResources>
    {
        [field: SerializeField] public EffectTypeConfig[] EffectTypeConfigs { get; private set; }
        [field: SerializeField] public ScoreDivisorRule[] Rules { get; private set; }
        private static ClanBuffResources instance;
        public static ClanBuffResources Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<ClanBuffResources>("ClanBuffResources");
                }
                return instance;
            }
        }

        public int GetBaseValueOf(EffectType type)
        {
            var effect = EffectTypeConfigs.FirstOrDefault(x => x.Type == type);
            if (effect == null) return 0;
            var rule = Rules.FirstOrDefault(x => x.ScoreThreshold >= effect.Value);
            if (rule == null) return 0;
            return effect.Value / rule.Divisor;
        }
        [Button]
        private void Export()
        {
            var path = Path.Combine(Application.dataPath, "Exports", "ClanBuffResources.bin");
            var writer = new Writer(ByteSize());
            writer.Write(this);
            File.WriteAllBytes(path, writer.Buffer);
            Debug.Log(path);
        }

        public void Deserialize(IReader reader, ref ClanBuffResources value)
        {
            value.EffectTypeConfigs = reader.ReadArray<EffectTypeConfig>();
            value.Rules = reader.ReadArray<ScoreDivisorRule>();
        }
        public int ByteSize()
        {
            return EffectTypeConfigs.ByteSize() + Rules.ByteSize();
        }
        public void Serialize(IWriter writer)
        {
            writer.WriteArray(EffectTypeConfigs);
            writer.WriteArray(Rules);
        }
    }
}
