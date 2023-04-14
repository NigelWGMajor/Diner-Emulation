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
            var summary = String.Join("\n", table.Diners.Select(d => (d.Name, d.Selection)).Select(s => $"{s.Item1}: {String.Join(", ", s.Item2.Select(x => x.ItemName))}").ToArray())+ "\n";            int requestId = (int) await _client.SpInsertAsync("Request_insert",
                 ("@origin", table.TableNumber),
                 ("@initiator", request.Initiator),
                 ("@contact", request.Contact), 
                 ("Request", summary)
                );
            int dinerIndex = 0;

            foreach (var diner in table.Diners)
            {
                int itemIndex = 0;
                foreach (var selection in diner.Selection)
                {
                    await CreateOperations(requestId, diner.TableNumber, dinerIndex, itemIndex++, selection.ItemName);
                }
                dinerIndex++;
            }
            return requestId;
        }
        // For each menu item, we create potential operations for that, one reaction per each.
        private async Task CreateOperations(long requestId, int tableNumber, int DinerNumber, int itemIndex, string MenuItem)
        {
            try
            {
                var ops = _client.SpAsType<Operation>("OperationsForMenuItem", ("@ItemName", MenuItem)).ToArray();

                foreach (var operation in ops)
                {
                    operation.Attempts = 0;
                    operation.RequestId = requestId;
                    operation.ErrorCount = 0;
                    operation.IsComplete = 0;
                    operation.DinerIndex = DinerNumber;
                    operation.ItemIndex = itemIndex;
                    await _client.InsertAsync("Operations", operation);
                }
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
