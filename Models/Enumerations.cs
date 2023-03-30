namespace Models;
public class Enumerations
{
    public enum ResultState
    {
        Waiting = -2,
        Errored = -1,
        Unknown = 0,
        Pending = 1,
        Started = 2,
        Resuming = 3,
        Completed = 4,
    }
    public enum ItemType
    {
        Entree,
        Side
    }
    [Flags]
    public enum OperationType
    {
        NOP = 0,
        Stock = 1,  // Get supplies into storage
        Fetch = 2,  // Get ingredients from supplies
        Prepare = 4,// wash, cut, whatever 
        Cook = 8,   // Fire
        Plate = 16,  // 
        Garnish = 32, // add finishing stuff
    }
}
