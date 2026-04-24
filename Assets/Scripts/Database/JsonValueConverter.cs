using System;
using Newtonsoft.Json;
using SQLite;
using UnityEngine;

namespace Database
{
    public class JsonValueConverter : IValueConverter
    {
        public object ToSQLiteValue(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public object FromSQLiteValue(object sqliteValue, Type targetType)
        {
            return JsonConvert.DeserializeObject(sqliteValue.ToString(), targetType);
        }

        public string GetSqlType()
        {
            const string type = "varchar";
            return type;
        }
    }
}