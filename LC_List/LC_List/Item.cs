using System.Collections.Generic;

namespace LC_List
{
    public class Item
    {
        public ItemCategory Name { get; set; }
        public int LCCondition { get; set; }
        public int StdType { get; set; }
        public string Config { get; set; }
        public List<Inj> Injs { get; set; }
    }
}