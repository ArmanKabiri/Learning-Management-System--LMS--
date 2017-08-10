using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VU.BLL
{
    public class Cache<TKey, TValue>
    {

        readonly Dictionary<TKey, TValue> _storage = new Dictionary<TKey, TValue>();

        public void Add(TKey key, TValue value)
        {
            _storage.Add(key, value);
        }

        public void Sort()
        {
            //_storage.o
        }

        public void Replace(TKey key, TValue value)
        {
            _storage[key] = value;
        }

        public TValue Get(TKey key)
        {
            return _storage[key];
        }


        public List<TValue> ToList()
        {
            List<TValue> all = new List<TValue>();
            return _storage.Values.ToList();
        }

        public bool Remove(TKey key)
        {
            if (_storage.ContainsKey(key))
                _storage.Remove(key);
            return true;
        }



        public bool ContainKey(TKey key)
        {
            if (_storage.ContainsKey(key))
                return true;
            else
                return false;
        }

        public List<TKey> GetAllKeys()
        {
            List<TKey> allKeys = new List<TKey>();
            return _storage.Keys.ToList();
        }


    }
}
