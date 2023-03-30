namespace Models;
public class Operation
{
    public string Name { get; set; }
    public OperationType OperationType { get; set; }
    public ResultState Execute();
}
