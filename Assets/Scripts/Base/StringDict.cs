using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Base
{
    [Serializable]
    public class StringDict
    {
        [field: SerializeField] public List<string> Dict { get; private set; }

        public int Add(string value)
        {
            var result = Dict.IndexOf(value);
            if (result == -1)
            {
                var index = Dict.Count;
                Dict.Add(value);
                return index;
            }
            return result;
        }
        public string this[int key] => Dict[key];

        [Button]
        public void Clear()
        {
            Dict = new();
        }
    }
}