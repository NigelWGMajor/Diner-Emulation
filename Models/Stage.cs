namespace Models
{
    public class Stage
    {
        public List<Reaction> Reactions {get; set; } = new();
        public Activity Activity { get; set; }
        public int CycleCount { get; set; }
    }
}