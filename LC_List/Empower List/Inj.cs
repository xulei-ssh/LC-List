using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;

namespace Empower_List
{
    public class Inj:INotifyPropertyChanged,IEqualityComparer<Inj>
    {
        private int vol;
        public int Volume
        {
            get
            {
                return vol;
            }
            set
            {
                vol = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Volume"));
            }
        }
        private int con;
        public int Count
        {
            get
            {
                return con;
            }
            set
            {
                con= value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
            }
        }
        private string nam;
        public string Name
        {
            get
            {
                return nam;
            }
            set
            {
                nam = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }
        private double tim;
        public double Time
        {
            get
            {
                return tim;
            }
            set
            {
                tim = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Time"));
            }
        }

        public Inj(int voulme, int count, string name, double time)
        {
            vol = voulme;
            con = count;
            nam = name;
            tim = time;
        }
        public Inj() : this(10, 1, "", 10) { }
        public event PropertyChangedEventHandler PropertyChanged;

        public bool Equals(Inj x, Inj y)
        {
            return x.nam == y.nam;
        }

        public int GetHashCode(Inj obj)
        {
            return base.GetHashCode();
        }
    }
}
