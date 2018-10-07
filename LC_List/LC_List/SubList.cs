using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LC_List
{
    public class SubList : IEnumerable<ListEntry>
    {
        public int StdType { get; set; }
        public List<ListEntry> SubEntryList { get; set; }

        public IEnumerator<ListEntry> GetEnumerator()
        {
            return SubEntryList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return SubEntryList.GetEnumerator();
        }
        public SubList()
        {
            SubEntryList = new List<ListEntry>();
        }
        public void Add(int vial, Inj inj)
        {
            SubEntryList.Add(new ListEntry(vial, inj));

        }


    }
}
