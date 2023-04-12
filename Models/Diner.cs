namespace Models;

public class Diner : Person
{

    public int TableNumber { get; set; }
    public List<MenuItem> Selection { get; set; } = new();
}
