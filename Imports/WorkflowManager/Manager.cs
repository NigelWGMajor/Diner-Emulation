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
        public async Task<int> RequestWork(WorkRequest request, Table table)
        {
            request.ReceivedAt = DateTime.Now;
            request.IsActive = 1;
            int requestId = await _client.InsertAsync("Requests", request);
            int dinerIndex = 0;
            foreach (var diner in table.Diners)
            {
                foreach (var selection in diner.Selection)
                {
                    CreateOperations(requestId, diner.TableNumber, dinerIndex, selection.ItemName);
                }
                dinerIndex++;
            }
            return requestId;
        }
        // For each menu item, we create potential operations for that, one reaction per each.
        private async Task CreateOperations(long requestId, int tableNumber, int DinerNumber, string MenuItem)
        {
            try
            {
                var ops = _client.SpAsJson("OperationsForMenuItem", ("@ItemName", MenuItem));

                //foreach (var operation in ops)
                //{
                //    operation.Attempts = 0;
                //    operation.RequestId = requestId;
                //    operation.ErrorCount = 0;
                //    operation.IsComplete = 0;
                //    await _client.InsertAsync("Operations", operation);












                //}
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
        }
        // The Executor claims a task: this stops other folks from using it, and gives a reference to the request.
        public async Task<WorkTask> ClaimTask(string Executor)
        {
            var data = _client.SpAsType<WorkTask>("ClaimTask", ("@Name", Executor));
            return await Task.FromResult(data.First());
        }
        // 
        // The Executor updates the status periodically
        /*public async Task UpdateTask(long Id, string outcome)
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
       */


    }
}
