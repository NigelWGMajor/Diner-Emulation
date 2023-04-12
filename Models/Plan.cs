namespace Models
{
    public class Plan : MenuItem
    {
        public Plan(string kind, string name, params Stage[] stages) :
            base(kind, name)
        {
            Stages = stages;
        }

        public Stage[] Stages { get; set; }

        public int AtStage { get; set; }

        public int CycleCount => Stages.Sum(s => s.CycleCount);

        public int StageCount => Stages.Count();

        public PlanStatus Status { get; set; } = PlanStatus.Pending;
    }
}
