using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Empower_List
{
    public class ProjectInfo
    {
        public string Protocol { get; set; }
        public List<Item> Items { get; set; }
        public Item this[string index]
        {
            get
            {
                return Items.Find(x => x.Name == index);
            }
        }
    }
}
