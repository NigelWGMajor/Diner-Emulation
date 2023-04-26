using WorkflowManager.Models;

namespace Models.Common
{
    public class Table : WorkflowManager.Models.IFlowManageable
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
        long IFlowManageable.OriginId { get => (long)TableNumber; }
        string IFlowManageable.Identifier { get => Server.Name;}
        IEnumerable<string> IFlowManageable.Items { get => Diners.Select(d => d.Name); }
        IEnumerable<IEnumerable<string>> IFlowManageable.Data { get => 
                Diners.Select(d => d.Selection.Select(s => s.ItemName).ToArray()); }

    }
}