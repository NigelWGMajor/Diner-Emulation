using System;
using Nix.Library.SqlStore;
using Models.Common;
using WorkflowManager.Models;
using System.Threading.Tasks;
using System.Linq;

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
        [Obsolete("Need to use RequestDeliverable")]
        public async Task<int> RequestWork(WorkRequest request, Table table)
        {
            request.ReceivedAt = DateTime.Now;
            request.IsActive = 1;
            var summary = String.Join("\n", table.Diners.Select(d => (d.Name, d.Selection)).Select(s => $"{s.Item1}: {String.Join(", ", s.Item2.Select(x => x.ItemName))}").ToArray()) + "\n";
            int requestId = (int)await _client.SpInsertAsync("Request_insert",
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
        // new version
        public async Task<int> RequestDeliverable(WorkRequest request, Table table)
        {
            request.ReceivedAt = DateTime.Now;
            request.IsActive = 1;
            var summary = String.Join("\n",
                table.Diners.Select(d => (d.Name, d.Selection))
                .Select(s => $"{s.Item1}: {String.Join(", ", s.Item2.Select(x => x.ItemName))}")
                .ToArray()) + "\n";
            int requestId = (int)await _client.SpInsertAsync("Request_insert",
                 ("@origin", table.TableNumber),
                 ("@initiator", request.Initiator),
                 ("@contact", request.Contact),
                 ("Request", summary)
                );
            int dinerIndex = 0;
            foreach (var diner in table.Diners)
            {
                int itemIndex = 0;
                foreach (var item in diner.Selection)
                {
                    int count = (int)await _client.SpInsertAsync("Deliverable_insert",
                        ("@requestId", requestId),
                        ("@dinerIndex", dinerIndex),
                        ("@itemIndex", itemIndex),
                        ("@itemName", item.ItemName)
                        );
                    itemIndex++;
                }
                dinerIndex++;
            }
            return requestId;
        }
        // executor claim responsibility and gets a deliverable ID to work on.
        public async Task<int> ClaimResponsibility(string executor) //! could also claim a particular type of deliverable....
        {
            int deliverableId = (int)await _client.SpExecuteAsync("Claim_Responsibility",
                ("@executor", executor)
                );
            return deliverableId;
        }
        public async Task<Attempt> GetNextOperationAsync(string executor)
        {
            await Task.CompletedTask;

            var result = _client.SpAsType<Attempt>("Operation_Next",
                ("@executor", executor)
               ).First();

            // need to get the next task from the data
            return result;
        }

        public async Task NotifyResultAsync(Attempt attempt)
        {
            if (!attempt.Succeeded)
            {
                // publish the failed state...
            }
        }
    }
}
