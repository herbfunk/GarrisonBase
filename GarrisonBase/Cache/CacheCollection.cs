using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Cache.Objects;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase.Cache
{
    public class CacheCollection : IDictionary<WoWGuid, C_WoWObject>
    {
        private readonly Dictionary<WoWGuid, C_WoWObject> _objects = new Dictionary<WoWGuid, C_WoWObject>(); 
        public IEnumerator<KeyValuePair<WoWGuid, C_WoWObject>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _objects.GetEnumerator();
        }

        public void Add(KeyValuePair<WoWGuid, C_WoWObject> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            _objects.Clear();
        }

        public bool Contains(KeyValuePair<WoWGuid, C_WoWObject> item)
        {
            return _objects.Contains(item);
        }

        public void CopyTo(KeyValuePair<WoWGuid, C_WoWObject>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<WoWGuid, C_WoWObject> item)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _objects.Values.Count; }
        }
        public bool IsReadOnly { get; private set; }
        public bool ContainsKey(WoWGuid key)
        {
            return _objects.ContainsKey(key);
        }

        public void Add(WoWGuid key, C_WoWObject value)
        {
            _objects.Add(key, value);
        }

        public bool Remove(WoWGuid key)
        {
            return _objects.Remove(key);
        }

        public bool TryGetValue(WoWGuid key, out C_WoWObject value)
        {
            return _objects.TryGetValue(key, out value);
        }
        public bool TryGetValue<T>(WoWGuid key, out T value) where T : C_WoWObject
        {
            if (_objects.ContainsKey(key))
            {
                value = (T)_objects[key];
                return true;
            }
            value = null;
            return false;
        }
        public C_WoWObject this[WoWGuid key]
        {
            get
            {
                return _objects[key];
            }
            set
            {
                _objects[key] = value;
            }
        }

        public ICollection<WoWGuid> Keys
        {
            get
            {
                return _objects.Keys;
            }
        }
        public ICollection<C_WoWObject> Values
        {
            get
            {
                return _objects.Values;
            }
        }

        public IEnumerable<C_WoWObject> GetObjectsOfSubType(WoWObjectTypes type)
        {
            foreach (var obj in _objects.Values)
            {
                if (ObjectCacheManager.CheckFlag(obj.SubType, type))
                    yield return obj;
            }
        }
        public IEnumerable GetObjectsOfType<T>() where T : C_WoWObject
        {
            foreach (var obj in _objects.Values)
            {
                if (obj is T)
                    yield return obj;
            }
        }
        ///<summary>
        ///Returns any objects found with matching TargetTypes.
        ///</summary>
        public IEnumerable<C_WoWUnit> Units()
        {
            foreach (var item in _objects.Values)
            {
                if (item is C_WoWUnit)
                    yield return (C_WoWUnit)item;
            }
        }

    }
}