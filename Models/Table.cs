namespace Models
{
    public class Table
    {
        static int _firstTable = 1;
        public Table(Server server)
	    {
		    TableNumber = _firstTable++;
		    Server = server;
		    Server.TableNumbers.Add(TableNumber);
	    }
        public void Seat(Diner diner)
        {
            diner.TableNumber = TableNumber;
        }
        public int TableNumber {get; set; }
        public Server Server {get; set;}
        public List<Diner> Diners {get; set; } = new();
    }
}