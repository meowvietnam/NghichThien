using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LongDH.LongDHModules
{
    public static class Pool
    {

        static readonly Dictionary<Type, object> m_Dict = new Dictionary<Type, object>();
        static readonly Dictionary<Type, Func<object>> m_DictCreate = new Dictionary<Type, Func<object>>();

        static Pool()
        {
        }
        public static void Set<T>(T t) where T : class, new()
        {
            if (t == null)
            {
                return;
            }
            var type = typeof(T);
            m_Dict[type] = t;
        }

        public static void Set(Type type, object obj)
        {
            if (type == null)
            {
                return;
            }
            m_Dict[type] = obj;
        }

        public static T Get<T>() where T : class, new()
        {
            var type = typeof(T);
            if (m_Dict.TryGetValue(type, out var result))
            {
                return result as T;
            }
            var t = new T();
            m_Dict[type] = t;
            return t;
        }
        public static void ClearAll()
        {
            m_Dict.Clear();
            m_DictCreate.Clear();
        }

        public static void ClearAllSingle()
        {
            m_Dict.Clear();
        }

        public static void ClearAllCreate()
        {
            m_DictCreate.Clear();
        }
    }
}

