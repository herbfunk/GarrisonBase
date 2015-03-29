using System;
using System.Collections;
using System.Collections.Generic;

namespace Herbfunk.GarrisonBase.Cache
{
    public class EntryList : IList<uint>
    {
        private List<uint> _ids = new List<uint>();

        public IEnumerator<uint> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public delegate void ItemAdded(uint item);
        public event ItemAdded OnItemAdded;

        public void Add(uint item)
        {
            _ids.Add(item);
            if (OnItemAdded != null) OnItemAdded(item);
        }

        public void Clear()
        {
            _ids.Clear();
        }

        public bool Contains(uint item)
        {
            return _ids.Contains(item);
        }

        public void CopyTo(uint[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public delegate void ItemRemoved(uint item);
        public event ItemRemoved OnItemRemoved;

        public bool Remove(uint item)
        {
            bool res = _ids.Remove(item);
            if (res)
            {
                if (OnItemRemoved != null) OnItemRemoved(item);
            }
            return res;
        }

        public int Count { get; private set; }
        public bool IsReadOnly { get; private set; }
        public int IndexOf(uint item)
        {
            return _ids.IndexOf(item);
        }

        public void Insert(int index, uint item)
        {
            _ids.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _ids.RemoveAt(index);
        }

        public uint this[int index]
        {
            get { return _ids[index]; }
            set { _ids[index] = value; }
        }
    }
}
