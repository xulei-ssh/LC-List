using System.Collections;
using System.Collections.Generic;

namespace Empower_List
{
    public class Item : IEnumerable<Inj>
    {
        public string Name { get; set; }
        public int LCCondition { get; set; }
        public int StdType { get; set; }
        public string Config { get; set; }
        public List<Inj> Injs { get; set; }
        public bool NewLine { get; set; } = false;
        public Inj this[int index]
        {
            get
            {
                return Injs[index];
            }
        }

        public IEnumerator<Inj> GetEnumerator()
        {
            return Injs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Injs.GetEnumerator();
        }
    }
}