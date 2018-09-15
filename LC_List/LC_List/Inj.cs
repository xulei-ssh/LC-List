namespace LC_List
{
    public class Inj
    {
        public int Volume { get; set; }
        public int Count { get; set; }
        public string Name { get; set; }
        public double Time { get; set; }
        public Inj(int voulme, int count, string name, double time)
        {
            Volume = voulme;
            Count = count;
            Name = name;
            Time = time;
        }
    }
}
