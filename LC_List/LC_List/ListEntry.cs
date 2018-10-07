using System;

namespace LC_List
{
    public class ListEntry : IComparable<ListEntry>
    {
        public int Vial { get; set; }
        public int Volume { get; set; }
        public int InjCount { get; set; }
        public string Name { get; set; }
        public double Time { get; set; }
        public ListEntry(int vial, int volume, int count, string name, double time)
        {
            Vial = vial;
            Volume = volume;
            InjCount = count;
            Name = name;
            Time = time;
        }
        public ListEntry(int vial, Inj inj)
        {
            Vial = vial;
            Volume = inj.Volume;
            InjCount = inj.Count;
            Name = inj.Name;
            Time = inj.Time;
        }

        public int CompareTo(ListEntry other)
        {
            if (Volume == other.Volume && InjCount == other.InjCount && Name == other.Name && Time == other.Time) return 0;
            return 1;
        }
    }
}
