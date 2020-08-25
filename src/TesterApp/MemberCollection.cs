using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TesterApp
{
    public class MemberCollection : IList<ITypeMember>
    {
        private List<ITypeMember> typeMembers = new List<ITypeMember>();
        
        public ITypeMember this[int index]
        {
            get => typeMembers[index];
            set 
            {
                ThrowIfDuplicate(value);
                typeMembers[index] = value;
            }
        }

        public int Count => typeMembers.Count;

        public bool IsReadOnly => (typeMembers as IList<ITypeMember>).IsReadOnly;

        public void Add(ITypeMember item)
        {
            if (null == item)
                throw new ArgumentNullException(nameof(item));

            ThrowIfDuplicate(item);

            typeMembers.Add(item);
        }

        private void ThrowIfDuplicate(ITypeMember item)
        {
            if (typeMembers.Any(member => member.Name == item.Name))
                throw new ArgumentException("Duplicate member identifier");
        }

        public void Clear() => typeMembers.Clear();

        public bool Contains(ITypeMember item) => typeMembers.Contains(item);

        public void CopyTo(ITypeMember[] array, int arrayIndex) => typeMembers.CopyTo(array, arrayIndex);

        public IEnumerator<ITypeMember> GetEnumerator() => typeMembers.GetEnumerator();

        public int IndexOf(ITypeMember item) => typeMembers.IndexOf(item);

        public void Insert(int index, ITypeMember item)
        {
            ThrowIfDuplicate(item);
            typeMembers.Insert(index, item);
        }

        public bool Remove(ITypeMember item) => typeMembers.Remove(item);

        public void RemoveAt(int index) => typeMembers.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => typeMembers.GetEnumerator();
    }
}
