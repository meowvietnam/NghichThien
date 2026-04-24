using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    public class Pool<T> : MonoBehaviour where T : MonoBehaviour
    {
        [field: SerializeField] public T Prefab { get; private set; }
        [field: SerializeField] public int PoolSize { get; private set; } = 10;

        readonly Queue<T> pool = new();
        public void Init()
        {
            pool.Clear();
            for (int i = 0; i < PoolSize; i++)
            {
                var obj = Instantiate(Prefab, transform);
                obj.gameObject.SetActive(false);
                pool.Enqueue(obj);
            }
        }
        public T Get()
        {
            if (pool.Count == 0)
            {
                var obj = Instantiate(Prefab, transform);
                obj.gameObject.SetActive(false);
                return obj;
            }
            var item = pool.Dequeue();
            item.gameObject.SetActive(true);
            return item;
        }
        public void Release(T item)
        {
            item.gameObject.SetActive(false);
            item.transform.SetParent(transform);
            pool.Enqueue(item);
        }
    }
}