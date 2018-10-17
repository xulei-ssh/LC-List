using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Empower_List
{
    public class TaskSet
    {
        public string Lot { get; set; }
        public bool[] Items { get; set; }
        public TaskSet(string lot, int itemCount,int indexOfContentUniformity)
        {
            Items = new bool[4] { false, false, false, false };
            Lot = lot;
            if (itemCount > 0)
            {
                Items[0] = true;
                if (itemCount > 1)
                {
                    Items[1] = true;
                    if (itemCount > 2)
                    {
                        Items[2] = true;
                        if (itemCount > 3)
                        {
                            Items[3] = true;
                        }
                    }
                }
            }
            if (lot.Contains("(") || lot.Contains("（"))
            {
                Items[indexOfContentUniformity] = false;
            }

        }

    }
    public class TaskSetComparer : IEqualityComparer<TaskSet>
    {
        public bool Equals(TaskSet x, TaskSet y)
        {
            return x.Lot == y.Lot;
        }

        public int GetHashCode(TaskSet obj)
        {
            return base.GetHashCode();
        }
    }

}
