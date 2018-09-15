using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LC_List
{
    public class TaskSet
    {
        public string Lot { get; set; }
        public bool Item1 { get; set; } = false;
        public bool Item2 { get; set; } = false;
        public bool Item3 { get; set; } = false;
        public bool Item4 { get; set; } = false;
        public TaskSet(string lot, int itemCount)
        {
            Lot = lot;
            if (itemCount > 0)
            {
                Item1 = true;
                if (itemCount > 1)
                {
                    Item2 = true;
                    if (itemCount > 2)
                    {
                        Item3 = true;
                        if (itemCount > 3)
                        {
                            Item4 = true;
                        }
                    }
                }
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
