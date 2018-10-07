using System.Collections.Generic;

namespace LC_List
{
    public class Item
    {
        public string Name { get; set; }
        public int LCCondition { get; set; }
        public int StdType { get; set; }
        public string Config { get; set; }
        public List<Inj> Injs { get; set; }
        public Inj this[int index]
        {
            get
            {
                return Injs[index];
            }
        }
    }
}