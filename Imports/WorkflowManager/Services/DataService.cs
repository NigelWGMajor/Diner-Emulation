using Nix.Library.Lean.Sql;
using WorkflowManager.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowManager.Services
{
    public interface IDataService
    {

    }
    public class DataService : IDataService
    {
        private DataStore _store;
        public DataService(string connectionString) 
        { 
            _store = new DataStore(connectionString);
        }
    }
}
