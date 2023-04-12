namespace Models
{
    public class Table
    {
        public int TableNumber {get; set; }
        public Server Server {get; set;}
        public List<Diner> Diners {get; set; } = new();
    }
}