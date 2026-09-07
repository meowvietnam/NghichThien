using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Base
{
    [Serializable]
    public class ListWrapper<T>
    {
        [field: SerializeField] public List<T> Items { get; private set; }
    }
}