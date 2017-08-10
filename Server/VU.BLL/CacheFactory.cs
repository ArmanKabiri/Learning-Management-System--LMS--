using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VU.BLL
{
    internal class CacheObject
    {
        public CacheObject(Type key, Type value, object cache)
        {
            KeyType = key;
            ValueType = value;
            Cache = cache;
        }

        public Type KeyType { get; set; }
        public Type ValueType { get; set; }
        public object Cache { get; set; }
    }

    public class CacheFactory
    {
        Dictionary<string, CacheObject> _Caches = new Dictionary<string, CacheObject>();
        static readonly CacheFactory _Instance = new CacheFactory();
        public static CacheFactory Instance { get { return _Instance; } }
        private int _Count = new int();
        public int Count { get { return _Count; } }

        static CacheFactory()
        {

        }

        private CacheFactory()
        {

        }

        public void CreateCache<TKey, TValue>(string name)
        {
            _Caches.Add(name, new CacheObject(typeof(TKey), typeof(TValue), new Cache<TKey, TValue>()));
            _Count++;
        }

        public TValue GetValue<TKey, TValue>(string name, TKey key)
        {
            if (key == null)
            {
                return default(TValue);
            }
            else if (ContainKey<TKey, TValue>(name, key))
                return (_Caches[name].Cache as Cache<TKey, TValue>).Get(key);
            else
                return default(TValue);
        }

        public bool ContainKey<TKey, TValue>(string name, TKey key)
        {
            if (Exist(name))
                return (_Caches[name].Cache as Cache<TKey, TValue>).ContainKey(key);
            else
                return false;
        }


        public List<TValue> GetAll<TKey, TValue>(string name)
        {
            if (Exist(name))
                return (_Caches[name].Cache as Cache<TKey, TValue>).ToList();
            else
                return null;
        }

        public List<TKey> GetAllKeys<TKey, TValue>(string name)
        {
            if (Exist(name))
                return (_Caches[name].Cache as Cache<TKey, TValue>).GetAllKeys();
            else
                return null;
        }

        public void Add<TKey, TValue>(string cacheName, TKey key, TValue value)
        {
            (_Caches[cacheName].Cache as Cache<TKey, TValue>).Add(key, value);
        }

        public void SortCache<TKey, TValue>(string cacheName, TKey key, TValue value)
        {
            (_Caches[cacheName].Cache as Cache<TKey, TValue>).Sort();
        }
        public void Replace<TKey, TValue>(string cacheName, TKey key, TValue value)
        {
            (_Caches[cacheName].Cache as Cache<TKey, TValue>).Replace(key, value);
        }

        public bool Exist(string cacheName)
        {
            return _Caches.ContainsKey(cacheName);
        }

        public void RemoveCache(string name)
        {
            _Caches.Remove(name);
            _Count--;
        }
        public bool RemoveCacheElement<TKey, TValue>(string cacheName, TKey key)
        {
            (_Caches[cacheName].Cache as Cache<TKey, TValue>).Remove(key);
            return true;
        }


    }
}