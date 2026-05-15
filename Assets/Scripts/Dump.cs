using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AllMaps.NgoDao;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Skill;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace DumpGame
{
    public static class ReflectionUtils
    {
        public static bool IsOverride(System.Type type, string methodName)
        {
            var method = type.GetMethod(
                methodName,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );
            if (method == null)
                return false;
            return method.GetBaseDefinition().DeclaringType != method.DeclaringType;
        }
    }

    public enum Type
    {
        NgoDao,
        CongPhap,
        ThanThong
    }

    public enum SerializeType
    {
        Normal,
        Array,
        List,
        String,
    }

    [Serializable]
    public class SerializeMember
    {
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public SerializeType SerializeType { get; set; }
        [field: SerializeField] public string Type { get; set; }
    }

    [Serializable]
    public class MetaData
    {
        [field: SerializeField] public Type Type { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string ClassName { get; set; }
        [field: SerializeField] public string ParentClassName { get; set; }
        [field: SerializeField] public string ScriptPath { get; set; }
        [field: SerializeField] public bool HasBuffValue { get; set; }
        [field: SerializeField] public List<SerializeMember> SerializeMembers { get; set; } = new();
    }

    [Serializable]
    public class MemberDefinition
    {
        [field: SerializeField] public string Type { get; set; }
        [field: SerializeField] public List<string> Members { get; set; } = new();
    }

    public class MemberInfo
    {
        public System.Type Type { get; set; }
        public string Name { get; set; }
    }

    public class Dump : MonoBehaviour
    {
        [SerializeField] private List<NgoDao> ngoDaos;
        [SerializeField] private List<CongPhap> congPhaps;
        [SerializeField] private List<ThanThong> thanThongs;
        [SerializeField] public List<MetaData> metaDatas;
        [SerializeField] public List<MemberDefinition> memberDefinitions;

        [Button("Refresh")]
        public void Refresh()
        {
            string[] guids = AssetDatabase.FindAssets("t:BaseData");
            Debug.Log($"Found {guids.Length} BaseData assets");
            var list = new List<BaseData>();
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                BaseData data = AssetDatabase.LoadAssetAtPath<BaseData>(path);
                list.Add(data);
            }

            ngoDaos = list.OfType<NgoDao>().ToList();
            congPhaps = list.OfType<CongPhap>().ToList();
            thanThongs = list.OfType<ThanThong>().ToList();
        }

        public string GetPath(Object obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            path = Path.GetFullPath(path);
            return path;
        }

        public string GetScriptPath(ScriptableObject so)
        {
            var monoScript = MonoScript.FromScriptableObject(so);
            var path = GetPath(monoScript);
            return path;
        }

        private List<MemberInfo> GetMemberInfos(System.Type classType)
        {
            var fields = classType.GetFields(
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.DeclaredOnly
            );
            var properties = classType.GetProperties(BindingFlags.Instance |
                                                     BindingFlags.Static |
                                                     BindingFlags.Public |
                                                     BindingFlags.NonPublic |
                                                     BindingFlags.DeclaredOnly);
            var members = fields.Select(f => new MemberInfo { Type = f.FieldType, Name = f.Name }).ToList();
            members.AddRange(properties.Select(f => new MemberInfo { Type = f.PropertyType, Name = f.Name }));
            return members;
        }

        public List<SerializeMember> GetMembers(System.Type classType)
        {
            var serializeMembers = new List<SerializeMember>();
            var definition = memberDefinitions.FirstOrDefault(x => x.Type == classType.Name);
            if (definition != null)
            {
                var members = GetMemberInfos(classType);
                members = members.Where(f => definition.Members.Contains(f.Name)).ToList();
                serializeMembers = members.Select(x =>
                {
                    SerializeType serializeType = SerializeType.Normal;
                    string type = x.Type.Name;
                    if (x.Type.IsArray)
                    {
                        serializeType = SerializeType.Array;
                        type = x.Type.GetElementType().Name;
                    }
                    else if (x.Type.IsGenericType)
                    {
                        var genericType = x.Type.GetGenericTypeDefinition();
                        if (genericType == typeof(List<>))
                        {
                            serializeType = SerializeType.List;
                            type = x.Type.GetGenericArguments()[0].Name;
                        }
                    }
                    else if (x.Type == typeof(string))
                    {
                        serializeType = SerializeType.String;
                    }

                    return new SerializeMember()
                    {
                        Name = x.Name,
                        SerializeType = serializeType,
                        Type = type,
                    };
                }).ToList();
            }

            return serializeMembers;
        }

        [Button]
        public void GetMemberDefinitions()
        {
            memberDefinitions = new();
            foreach (var ngoDao in ngoDaos)
            {
                if (!(ngoDao is NgoDaoBuffStats))
                {
                    continue;
                }

                var type = ngoDao.GetType();
                if (memberDefinitions.Any(x => x.Type == type.Name))
                {
                    continue;
                }
                var members = GetMemberInfos(type);
                members = members.ToList();
                var definition = new MemberDefinition()
                {
                    Type = type.Name,
                    Members = members.Select(x => x.Name).ToList(),
                };
                memberDefinitions.Add(definition);
            }
            foreach (var congPhap in congPhaps)
            {
                var type = congPhap.GetType();
                if (memberDefinitions.Any(x => x.Type == type.Name))
                {
                    continue;
                }
                var hasBuffValue = ReflectionUtils.IsOverride(type, "EquipCongPhap");
                if (!hasBuffValue)
                {
                    continue;
                }
                var members = GetMemberInfos(type);
                members = members.ToList();
                var definition = new MemberDefinition()
                {
                    Type = type.Name,
                    Members = members.Select(x => x.Name).ToList(),
                };
                memberDefinitions.Add(definition);
            }
        }

        public MetaData GetMetaData(ScriptableObject item)
        {
            var type = Type.CongPhap;
            switch (item)
            {
                case CongPhap cp:
                    type = Type.CongPhap;
                    break;
                case ThanThong th:
                    type = Type.ThanThong;
                    break;
                case NgoDao nd:
                    type = Type.NgoDao;
                    break;
            }

            var classType = item.GetType();
            var parentClassType = classType.BaseType;
            var hasBuffValue = false;
            switch (type)
            {
                case Type.CongPhap:
                    hasBuffValue = ReflectionUtils.IsOverride(classType, "EquipCongPhap");
                    break;
                case Type.ThanThong:
                    break;
                case Type.NgoDao:
                    hasBuffValue = item is NgoDaoBuffStats;
                    break;
            }

            var serializeMembers = GetMembers(classType);
            var metaData = new MetaData()
            {
                Type = type,
                Name = item.name,
                ClassName = classType.Name,
                ParentClassName = parentClassType?.Name,
                HasBuffValue = hasBuffValue,
                ScriptPath = GetScriptPath(item),
                SerializeMembers = serializeMembers,
            };
            return metaData;
        }

        [Button]
        public void DumpAll()
        {
            metaDatas = ngoDaos.Select(x => GetMetaData(x))
                .Concat(congPhaps.Select(y => GetMetaData(y)))
                .Concat(thanThongs.Select(z => GetMetaData(z)))
                .ToList();
            metaDatas = metaDatas
                .GroupBy(x => x.ScriptPath)
                .Select(x => x.First())
                .ToList();
            var json = JsonConvert.SerializeObject(metaDatas, Formatting.Indented);
            File.WriteAllText(Path.Combine(Application.dataPath,"Exports", "metadatas.json"), json);
        }
    }
}