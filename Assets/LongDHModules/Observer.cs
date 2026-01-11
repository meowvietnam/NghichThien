using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LongDH.LongDHModules
{
    public static class Observer
    {
        public delegate void CallbackObserverParam(params object[] data);

        public delegate void CallbackObserverVoid();

        private static readonly Dictionary<string, HashSet<CallbackObserverParam>> CallbackPara = new Dictionary<string, HashSet<CallbackObserverParam>>();

        private static readonly Dictionary<string, HashSet<CallbackObserverVoid>> CallbackVoid = new Dictionary<string, HashSet<CallbackObserverVoid>>();

        public static void AddObserver(string eventName, CallbackObserverVoid callback)
        {
            Debug.Log(eventName);

            CreateListObserverForEventVoid(eventName).Add(callback);
        }

        public static void AddObserver(string eventName, CallbackObserverParam callbackObserverParam)
        {
            Debug.Log(eventName);

            CreateListObserverForEventParam(eventName).Add(callbackObserverParam);
        }
      
        public static void RemoveObserver(string eventName, CallbackObserverVoid callback)
        {
            var listObserver = CreateListObserverForEventVoid(eventName);
            listObserver.Remove(callback);
        }

        public static void RemoveObserver(string eventName, CallbackObserverParam callbackObserverParam)
        {
            var listObserver = CreateListObserverForEventParam(eventName);
            listObserver.Remove(callbackObserverParam);
        }

        public static void RemoveAllObserver()
        {
            CallbackPara.Clear();
            CallbackVoid.Clear();
        }

        public static void Notify(string eventName, params object[] data)
        {
            // Tạo bản sao bằng .ToList() để tránh lỗi "Collection was modified"
            var listParam = CreateListObserverForEventParam(eventName).ToList();
            foreach (var observer in listParam)
            {
                observer?.Invoke(data);
            }

            // Tương tự cho danh sách Void
            var listVoid = CreateListObserverForEventVoid(eventName).ToList();
            foreach (var observer in listVoid)
            {
                observer?.Invoke();
            }
        }

        public static void Notify(string eventName)
        {
            if (CallbackPara.TryGetValue(eventName, out HashSet<CallbackObserverParam> paramCallbacks))
            {
                Debug.Log("paramcall back count "+ paramCallbacks.Count);
            }
            if (CallbackVoid.TryGetValue(eventName, out HashSet<CallbackObserverVoid> voidCallbacks))
            {
                Debug.Log("void back count " + voidCallbacks.Count);

            }

            foreach (var observer in CreateListObserverForEventParam(eventName))
                observer();
            foreach (var observer in CreateListObserverForEventVoid(eventName))
                observer();
        }

        private static HashSet<CallbackObserverParam> CreateListObserverForEventParam(string eventName)
        {
            if (!CallbackPara.ContainsKey(eventName))
                CallbackPara.Add(eventName, new HashSet<CallbackObserverParam>());
            return CallbackPara[eventName];
        }

        private static HashSet<CallbackObserverVoid> CreateListObserverForEventVoid(string eventName)
        {
            if (!CallbackVoid.ContainsKey(eventName))
                CallbackVoid.Add(eventName, new HashSet<CallbackObserverVoid>());
            return CallbackVoid[eventName];
        }

    }

}
