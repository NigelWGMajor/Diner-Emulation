using static Models.Enumerations;
namespace Models;

public class Operation
{
    public string Name { get; set; }
    public OperationType OperationType { get; set; }
    public Func<Item, ResultState> Executor { get; set; }

}
