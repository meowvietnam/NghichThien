using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Base
{
    public static class JsonUtils
    {
        private static readonly JsonSerializerSettings defaultSettings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new PrivateResolver(),
            Formatting = Formatting.Indented,
        };
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, defaultSettings);
        }
        public static T Parse<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json, defaultSettings);
        }
    }
}
