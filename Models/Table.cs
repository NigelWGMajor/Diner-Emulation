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
        public List<Diner> Diners
        {
            set 
            { 
                foreach(var diner in value)
                {
                    foreach(var menuItem in diner.Selection)
                    {
                        _items.Add(new WorkItem
                        {
                            Name = menuItem.ItemName,
                            Data = menuItem.ItemKind
                        });
                        _data.Add(diner.Name);
                    }
                }
            }
        }
        private List<string> _data = new List<string>();
        private List<IWorkItem> _items = new List<IWorkItem>();
        public IEnumerable<IWorkItem> WorkItems { get => _items; } 
        public IEnumerable<string> Data { get => _data; }   
        public CompletionState Completion { get; set; }
        public string Name { get => $"Table {TableNumber}";  }
    }
}