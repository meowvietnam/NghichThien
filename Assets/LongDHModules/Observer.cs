using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LongDH.LongDHModules
{
    public static class Observer
    {
        public delegate void CallbackObserverParam(params object[] data);

        public delegate void CallbackObserverVoid();

        private class ObserverItem<T>
        {
            public int Priority;
            public long OrderId;
            public T Callback;
        }

        private static long _globalOrderId = 0;

        private static readonly Dictionary<string, List<ObserverItem<CallbackObserverParam>>> CallbackPara
            = new Dictionary<string, List<ObserverItem<CallbackObserverParam>>>();

        private static readonly Dictionary<string, List<ObserverItem<CallbackObserverVoid>>> CallbackVoid
            = new Dictionary<string, List<ObserverItem<CallbackObserverVoid>>>();

        #region ADD OBSERVER

        public static void AddObserver(string eventName, CallbackObserverVoid callback, int priority = 0)
        {
            var list = CreateListObserverForEventVoid(eventName);

            if (list.Any(x => x.Callback == callback))
                return;

            list.Add(new ObserverItem<CallbackObserverVoid>
            {
                Callback = callback,
                Priority = priority,
                OrderId = _globalOrderId++
            });

            SortList(list);
        }

        public static void AddObserver(string eventName, CallbackObserverParam callback, int priority = 0)
        {
            var list = CreateListObserverForEventParam(eventName);

            if (list.Any(x => x.Callback == callback))
                return;

            list.Add(new ObserverItem<CallbackObserverParam>
            {
                Callback = callback,
                Priority = priority,
                OrderId = _globalOrderId++
            });

            SortList(list);
        }

        #endregion

        #region REMOVE

        public static void RemoveObserver(string eventName, CallbackObserverVoid callback)
        {
            if (CallbackVoid.TryGetValue(eventName, out var list))
            {
                list.RemoveAll(x => x.Callback == callback);
            }
        }

        public static void RemoveObserver(string eventName, CallbackObserverParam callback)
        {
            if (CallbackPara.TryGetValue(eventName, out var list))
            {
                list.RemoveAll(x => x.Callback == callback);
            }
        }

        public static void RemoveObserver(string eventName)
        {
            if (CallbackPara.TryGetValue(eventName, out var list))
            {
                list.Clear();
            }

            if (CallbackVoid.TryGetValue(eventName, out var voidList))
            {
                voidList.Clear();
            }
        }

        public static void RemoveAllObserver()
        {
            CallbackPara.Clear();
            CallbackVoid.Clear();
        }

        // Thêm vào trong class Observer
        public static void RemoveObserversForEvents(IEnumerable<string> eventNames)
        {
            foreach (var eventName in eventNames)
            {
                if (CallbackPara.ContainsKey(eventName))
                {
                    CallbackPara.Remove(eventName);
                }

                if (CallbackVoid.ContainsKey(eventName))
                {
                    CallbackVoid.Remove(eventName);
                }
            }
        }

        #endregion

        #region NOTIFY

        public static void Notify(string eventName, params object[] data)
        {
            if (CallbackPara.TryGetValue(eventName, out var paramList))
            {
                foreach (var item in paramList.ToList())
                    item.Callback?.Invoke(data);
            }

            if (CallbackVoid.TryGetValue(eventName, out var voidList))
            {
                foreach (var item in voidList.ToList())
                    item.Callback?.Invoke();
            }
        }

        public static void Notify(string eventName)
        {
            if (CallbackPara.TryGetValue(eventName, out var paramList))
            {
                foreach (var item in paramList.ToList())
                    item.Callback?.Invoke();
            }

            if (CallbackVoid.TryGetValue(eventName, out var voidList))
            {
                foreach (var item in voidList.ToList())
                    item.Callback?.Invoke();
            }
        }

        #endregion

        #region PRIVATE

        private static void SortList<T>(List<ObserverItem<T>> list)
        {
            list.Sort((a, b) =>
            {
                int priorityCompare = b.Priority.CompareTo(a.Priority);
                if (priorityCompare != 0)
                    return priorityCompare;

                return a.OrderId.CompareTo(b.OrderId);
            });
        }

        private static List<ObserverItem<CallbackObserverParam>> CreateListObserverForEventParam(string eventName)
        {
            if (!CallbackPara.ContainsKey(eventName))
                CallbackPara[eventName] = new List<ObserverItem<CallbackObserverParam>>();

            return CallbackPara[eventName];
        }

        private static List<ObserverItem<CallbackObserverVoid>> CreateListObserverForEventVoid(string eventName)
        {
            if (!CallbackVoid.ContainsKey(eventName))
                CallbackVoid[eventName] = new List<ObserverItem<CallbackObserverVoid>>();

            return CallbackVoid[eventName];
        }

        #endregion
    }
}