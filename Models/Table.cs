using WorkflowManager;
using WorkflowManager.Models;

namespace Models.Common
{
    public class Table : IActivation
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
        public IEnumerable<IWorkItem> Items { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IEnumerable<string> Data { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public CompletionState Completion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Name { get; set; }
    }
}