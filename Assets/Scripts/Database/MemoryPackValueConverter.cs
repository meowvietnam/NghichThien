using System;
using MemoryPack;
using SQLite;

namespace Database
{
    public class MemoryPackValueConverter<T> : IValueConverter
    {
        public object ToSQLiteValue(object value)
        {
            if (value is not T typedValue)
            {
                throw new ArgumentException($"Expected {typeof(T)}, but received {value?.GetType()}.", nameof(value));
            }

            return MemoryPackSerializer.Serialize(typedValue);
        }

        public object FromSQLiteValue(object sqliteValue, Type targetType)
        {
            if (sqliteValue is not byte[] bytes)
            {
                throw new ArgumentException($"Expected a SQLite BLOB for {targetType}.", nameof(sqliteValue));
            }

            return MemoryPackSerializer.Deserialize<T>(bytes);
        }

        public string GetSqlType()
        {
            return "blob";
        }
    }
}
