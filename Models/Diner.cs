namespace Models;

public class Diner
{
    public string Name { get; set; }
    public int TableNumber { get; set; }
    public List<MenuItem> Selection { get; set; } = new();
}
