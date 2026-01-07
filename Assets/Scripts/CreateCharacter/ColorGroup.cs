using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class ColorGroup
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public List<string> CategoryIds { get; private set; }
    }
}
