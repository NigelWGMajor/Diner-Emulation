namespace Models.Common
{
    public class Stage
    {
        public Stage(string name, string activity, int Cycles, params Reaction[] reactions)
	{
		Activity = activity;
		Reactions = reactions;
	}
        public Reaction[] Reactions {get; set; }
        public string Activity { get; set; }
        public int CycleCount { get; set; }
    }
}