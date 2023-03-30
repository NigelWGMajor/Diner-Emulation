using static Models.Enumerations;

namespace Models;
public class ProductionPlan
{
    public OperationType RequiredSteps { get; set; }
    public int CookTime { get; set; }

}
