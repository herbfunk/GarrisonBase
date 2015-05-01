using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Herbfunk.GarrisonBase.Cache.Objects;

namespace Herbfunk.GarrisonBase.Cache
{
    public class EntryCollection : IDictionary<uint, EntryBase>
    {
        private readonly Dictionary<uint, EntryBase> _entries = new Dictionary<uint, EntryBase>();

        public void Add(uint key, EntryBase value)
        {
            if (!_entries.ContainsKey(key))
                _entries.Add(key, value);
            else
                this[key] = value;
        }

        public void Add(EntryBase value)
        {
            if (!_entries.ContainsKey(value.Entry))
                _entries.Add(value.Entry, value);
            else
                this[value.Entry] = value;
        }

        public IEnumerator<KeyValuePair<uint, EntryBase>> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<uint, EntryBase> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            _entries.Clear();
        }

        public bool Contains(KeyValuePair<uint, EntryBase> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<uint, EntryBase>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<uint, EntryBase> item)
        {
            throw new NotImplementedException();
        }

        public int Count { get { return _entries.Count; }}
        public bool IsReadOnly { get; private set; }
        public bool ContainsKey(uint key)
        {
            return _entries.ContainsKey(key);
        }

        public bool Remove(uint key)
        {
            _entries.Remove(key);
            return true;
        }

        public bool TryGetValue(uint key, out EntryBase value)
        {
            return _entries.TryGetValue(key, out value);
        }

        public EntryBase this[uint key]
        {
            get
            {
                if (ContainsKey(key))
                {
                    //_entries[key].LastUsed = DateTime.Now;
                    return _entries[key]; //return reference
                }

                return null;
            }
            set
            {
                if (!ContainsKey(key))
                    //create with data given
                    _entries.Add(key, value);
                else
                {
                    //copy value
                    _entries[key] = value;
                    //_entries[key].LastUsed = DateTime.Now; //update
                }
            }
        }

        public ICollection<uint> Keys
        {
            get { return _entries.Keys; }
        }

        public ICollection<EntryBase> Values
        {
            get { return _entries.Values; }
        }
    }
}
