using System.Collections.Generic;

namespace Psy.Core
{
    public class FixedLengthList<T> : List<T>
    {
        private int _maxCount;
        public int MaxCount
        {
            get { return _maxCount; }
            set
            {
                if (_maxCount != value)
                {
                    Truncate(value);
                }
                _maxCount = value;
            }
        }

        public void Truncate(int value)
        {
            if (Count <= value)
                return;

            RemoveRange(value, Count - value);
        }

        public FixedLengthList() : base(5)
        {
            MaxCount = 0;
        }

        public FixedLengthList(int capacity) : base(capacity)
        {
            MaxCount = capacity;
        }

        public new void Add(T item)
        {
            if (Count == MaxCount && MaxCount > 0)
            {
                RemoveAt(0);
            }
            base.Add(item);
        }

        public new void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }
    }
}