using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Empower_List
{
    public class Item : IEnumerable<Inj>
    {
        public string Name { get; set; }
        public int LCCondition { get; set; }
        public int StdType { get; set; }
        public ObservableCollection<Inj> Injs { get; set; }
        public bool NewLine { get; set; } = false;
        public Inj this[int index]
        {
            get
            {
                return Injs[index];
            }
            set
            {
                Injs[index] = value;
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
        public Item() : this("", 0, 0) { }
        public Item(string name,int lcCondition,int stdType)
        {
            Name = name;
            LCCondition = lcCondition;
            StdType = stdType;
            Injs = new ObservableCollection<Inj>();
        }
        public void AddInj(Inj inj)
        {
            Injs.Add(inj);
        }
        public void DeleteInj(int index)
        {
            Injs.RemoveAt(index);
        }
        public List<Inj> FindAll(bool isSP)
        {
            List<Inj> r = new List<Inj>();
            foreach (var inj in Injs)
            {
                if ((isSP && inj.Name.Contains("sp") && isSP) || (!isSP && !inj.Name.Contains("sp")))
                {
                    r.Add(inj);
                }                
            }
            return r;
        }
    }
}