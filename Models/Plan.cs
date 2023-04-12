namespace Models
{
    public class Plan : MenuItem
    {
        public List<Stage> Stages { get; set; } = new List<Stage>();
        public int AtStage { get; set; }
        public int CycleCount => Stages.Sum(s => s.CycleCount);
        public int StageCount => Stages.Count();
        public PlanStatus Status { get; set; } = PlanStatus.Pending;
    }
}
