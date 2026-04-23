using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenKhi
{
    [Serializable]
    public class Material
    {
        [field: SerializeField,TabGroup("Info")]
        public int Id { get; set; }
        [field: SerializeField,TabGroup("Info")]
        public string Name { get; set; }
        [field: SerializeField,TabGroup("Info")]
        public Tier Tier { get; set; }
        [field: SerializeField, PreviewField(75),TabGroup("Info")]
        public Sprite Icon { get; set; }
        [field: SerializeField,TabGroup("Info")]
        public CultivationElement CultivationElement { get; set; }
        [field: SerializeField,TabGroup("Info")]
        public YinYangType YinYangType { get; set; }
        [field: SerializeField,TabGroup("Ref")]
        public int TypeRef { get; set; }
        [field: SerializeField,TabGroup("Ref")]
        public int PropertyRef { get; set; }
        [field: SerializeField,TabGroup("Ref")]
        public int[] OriginRef { get; set; }
        [field: SerializeField,TabGroup("Ref")]
        public int Desc1Ref { get; set; }
        [field: SerializeField,TabGroup("Ref")]
        public int Desc2Ref { get; set; }
        [field: SerializeField,TabGroup("Data")]
        public int[] ItemFlags { get; set; }
        [field: SerializeField,TabGroup("Data")]
        public int Price { get; set; }
        private StringDict Dict => LuyenKhiResources.Instance.Strings;
        [TabGroup("Preview"), ShowInInspector] public string Type => Dict[TypeRef];
        [TabGroup("Preview"), ShowInInspector] public string Property => Dict[PropertyRef];
        [TabGroup("Preview"), ShowInInspector] public string[] Origin => OriginRef.Select(o => Dict[o]).ToArray();
        [TabGroup("Preview"), ShowInInspector] public string Desc1 => Dict[Desc1Ref];
        [TabGroup("Preview"), ShowInInspector] public string Desc2 => Dict[Desc2Ref];
    }
}