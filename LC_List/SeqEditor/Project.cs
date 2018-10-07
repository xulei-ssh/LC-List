using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeqEditor
{
    class Project:IEnumerable<Item>
    {
        public string Name { get; set; }
        public string Protocol { get; set; }
        public List<Item> Items { get; set; }

        public IEnumerator<Item> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
