using System;
using System.ComponentModel.Design;
using Nix.Library.SqlStore;
using Models.Common;
using WorkflowManager.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Numerics;
using System.Net.Sockets;

namespace WorkflowManager
{
    public class Manager
    {
        private SqlClient _client;
        private string _connection = Environment.GetEnvironmentVariable("NIX_DB");
        public Manager()
        {
            Initialize();
        }
        private void Initialize()
        {
            InitializeDatabase();
        }
        private void InitializeDatabase()
        {
            _client = new SqlClient(_connection);
        }
        // The Initiator Places the work request 
        public async Task<int> RequestWork(WorkRequest request)
        {
            request.ReceivedAt = DateTime.Now;
            request.IsActive = 1;
            int id = await _client.InsertAsync("Requests", request);
            int d = 0;
            foreach (var diner in request.Table.Diners)
            {
                foreach (var selection in diner.Selection)
                {
                    CreateOperations(id, diner.TableNumber, d, selection.ItemName);
                }
                d++;
            }
            return id;
        }
        // For each menu item, we create potential operations for that, one reactiion per each.
        private void CreateOperations(long requestId, int tableNumber, int DinerNumber, string MenuItem)
        {
            var ops = _client.SpAsType<Operation>("OperationsForMenuItem", ("@ItemName", MenuItem));
        }
        // The Executor claims a task
        public async Task<WorkTask> ClaimTask(string Executor)
        {
            var data = _client.SpAsType<WorkTask>("ClaimTask", ("@Name", Executor));
            await GenerateOperations(data.First().Id);
            return await Task.FromResult(data.First());
        }
        // 
        // The Executor updates the status periodically
        public async Task UpdateTask(long Id, string outcome)
        {
            var _ = _client.SpAsType<WorkTask>("UpdateTask", ("@outcome", outcome));
        }
        // The Executor finalizes the task
        public async Task CloseTask(long Id, string outcome, int churn)
        {
            var _ = _client.SpAsType<WorkTask>("FinalizeTask", ("@outcome", outcome), ("@churn", churn));
            await Task.CompletedTask;
        }
        // The Initiator retrieves status
        public async Task GetTaskStatus(long Id)
        {

        }
        private async Task GenerateOperations(long Id)
        {

        }


    }
}
