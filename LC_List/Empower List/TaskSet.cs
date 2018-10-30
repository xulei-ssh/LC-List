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
        public TaskSet(string lot, int itemCount)
        {
            Items = new bool[4] { true, true, true, true };
            Lot = lot;
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
