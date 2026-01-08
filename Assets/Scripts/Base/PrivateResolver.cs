using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Base
{
    public class PrivateResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(
            Type type, MemberSerialization memberSerialization)
        {
            var props = type.GetFields(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Select(f => base.CreateProperty(f, memberSerialization))
                .ToList();

            props.ForEach(p =>
            {
                p.Readable = true;
                p.Writable = true;
            });

            return props;
        }
    }
}
