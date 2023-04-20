using System;
using Nix.Library.SqlStore;
using Models.Common;
using WorkflowManager.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Emulator.Models.Log;
using Emulator.Models;
using Models;

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
        //[Obsolete("Need to use RequestDeliverable")]
        //public async Task<int> RequestWork(WorkRequest request, Table table)
        //{
        //    request.ReceivedAt = DateTime.Now;
        //    request.IsActive = 1;
        //    var summary = String.Join("<br>", table.Diners.Select(d => (d.Name, d.Selection)).Select(s => $"{s.Item1}: {String.Join(", ", s.Item2.Select(x => x.ItemName))}").ToArray()) + "<br>";
        //    int requestId = (int)await _client.SpInsertAsync("Request_insert",
        //         ("@origin", table.TableNumber),
        //         ("@initiator", request.Initiator),
        //         ("@contact", request.Contact),
        //         ("Request", summary)
        //        );
        //    int dinerIndex = 0;

        //    foreach (var diner in table.Diners)
        //    {
        //        int itemIndex = 0;
        //        foreach (var selection in diner.Selection)
        //        {
        //            await CreateOperations(requestId, diner.TableNumber, dinerIndex, itemIndex++, selection.ItemName);
        //        }
        //        dinerIndex++;
        //    }

        //    LoggedEvents.Insert(0, new EventLogItem
        //    {
        //        Content = $"Ordered: {String.Join("\n", table.Diners.Select(d => (d.Name, d.Selection)).Select(s => $"{s.Item1}: {String.Join(", ", s.Item2.Select(x => x.ItemName))}").ToArray())}",
        //        EventTime = DateTime.Now
        //    });
        //    return requestId;
        //}
        // For each menu item, we create potential operations for that, one reaction per each.
        //private async Task CreateOperations(long requestId, int tableNumber, int DinerNumber, int itemIndex, string MenuItem)
        //{
        //    try
        //    {
        //        var ops = _client.SpAsType<Operation>("OperationsForMenuItem", ("@ItemName", MenuItem)).ToArray();

        //        foreach (var operation in ops)
        //        {
        //            operation.Attempts = 0;
        //            operation.RequestId = requestId;
        //            operation.ErrorCount = 0;
        //            operation.IsComplete = 0;
        //            operation.DinerIndex = DinerNumber;
        //            operation.ItemIndex = itemIndex;
        //            await _client.InsertAsync("Operations", operation);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await Console.Out.WriteLineAsync(ex.Message);
        //    }
        //}
        // new version
        public async Task<int> RequestDeliverableAsync(WorkRequest request, Table table)
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
            LoggedEvents.Insert(0, new EventLogItem
            {
                Content = $"Ordered: {String.Join("\n", table.Diners.Select(d => (d.Name, d.Selection)).Select(s => $"{s.Item1}: {String.Join(", ", s.Item2.Select(x => x.ItemName))}").ToArray())}",
                EventTime = DateTime.Now
            });
            return requestId;
        }
        // executor claim responsibility and gets a deliverable ID to work on.
        public async Task<int> ClaimResponsibilityAsync(string executor) //! could also claim a particular type of deliverable....
        {
            int deliverableId = (int)await _client.SpExecuteAsync("Claim_Responsibility",
                ("@executor", executor)
                );
            if (deliverableId > 0)
            LoggedEvents.Insert(0, new EventLogItem { 
                EventClass="log-blue", Content = $"{executor} Claimed Responsibility" });
            return deliverableId;
        }
        public async Task<Attempt> GetNextOperationAsync(string executor)
        {
            await Task.CompletedTask;

            var result = _client.SpAsType<Attempt>("Operation_Next",
                ("@executor", executor)
               ).FirstOrDefault();
            if (result != null)
                LoggedEvents.Insert(0, new EventLogItem
                {
                    EventClass = "log-green",
                    Content = $"Next operation for {executor}: {result.OperationName}"
                });
            else
                LoggedEvents.Insert(0, new EventLogItem
                {
                    EventClass = "log-blue",
                    Content = $"No pending operations."
                });

            return result;
        }
        public IEnumerable<EventLogItem> GetEvents()
        {
            var result = LoggedEvents.Take(20).ToArray();
            return result;
        }

        private static List<EventLogItem> LoggedEvents { get; set; } = new List<EventLogItem>();
        public async Task NotifyResultAsync(Attempt attempt)
        {
            string x = "log-yellow";
            bool showMessage= false;
            if (attempt.Outcome == "Succeeded")
                x = "log-green";
            if (attempt.Outcome == "Failed")
            {
                x = "log-red";
                showMessage = true;
            }
            await _client.SpExecuteAsync("Attempt_Update", ("@deliverableId", attempt.DeliverableId), ("@outcome", attempt.Outcome));

            LoggedEvents.Insert(0, new EventLogItem
            {
                Content = $"{attempt.OperationName}: {attempt.Outcome}. {(showMessage ? attempt.Message : "" )} ",
                EventClass = x,
                EventTime = DateTime.Now
            });
        }
        
        public async Task<Statistics>GetStatistics()
        {
            await Task.CompletedTask;
            return _client.SpAsType<Statistics>("Operations_Summary").First();    
        }
    }
}
