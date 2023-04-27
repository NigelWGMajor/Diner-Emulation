using System;
using WorkflowManager.Models;
using System.Threading.Tasks;
using System.Linq;
using WorkflowManager.Services;
using System.Collections;
using System.Collections.Generic;

namespace WorkflowManager
{
    [Flags]
    public enum CompletionState
    {   // need to be simple but useful
        Pending = 0,     // waiting for service
        Active = 1,      // is started, actively being worked 
        Retrying = 2,    // had errors but not fatal
        Cancelled = 32,  // future?
        Failed = 64,     // completed but unsuccessful
        Succeeded = 128, // Completed, done
        Archived = 256   // retired to storage
    }
    public interface IFlowManager
    {
        Task<IActivation> ActivateAsync(IActivation activation); // Consumer Activates a request
        Task<IOperable> GetNextAsync(IAbility ability = null);          // Provider gets operation based on ability
        Task<IOperable> UpdateAsync(IOperable operation);        // Update final or intermediate status or detect cancellation
        Task<IActivation> DeliverAsync(IActivation activation);  // Consumer retrieves final state/status
        Task<List<FlowLogItem>> GetLog(int count = 100, int minSeverity = 3);  // Retrieve recent events 
        Task<FlowMetrics> GetMetrics(); // Get the prevailing state of the activations
    }
    public interface IActivation       // The information needed to start or end a flow.
    {
        IEnumerable<IItem> Items { get; set; }   // the items that make up this set
        IEnumerable<string> Data { get; set; }   // json data
        CompletionState Completion { get; set; } // progress?
    }
    public class FlowLogItem
    {
        public string Content { get; set; }
        /// <summary>
        /// 0 = Informational/blue 1 = Good/green 2 = Warn/yellow 3 = Error/Red
        /// </summary>
        public int Severity { get; set; }
        public DateTime EventTime { get; set; }
    }
    public class FlowMetrics
    {
        public int ActivationsPending { get; set; }
        public int ActivationsActive { get; set; }
        public int ActivationsRetrying { get; set; }
        public int ActivationsFailed { get; set; }
        public int ActivationsSucceeded { get; set; }
        public int ActivationsArchived { get; set; }
    }
    public interface IItem           // identifies a single thing that has a single defined workflow
    {
        string Name { get; set; }    //  important, because this defines the workflow rules
        string Data { get; set; }   // json payload
    }
    public interface IOperable       // Information for the immediate task to be done 
    {
        string OperationName { get; set; } // to direct to the correct function
        CompletionState Completion { get; set; }  // how complete this is (for progress)

        string Data { get; set; }
    }
    public interface IAbility        // Qualifications for types of operation
    {
        string Name { get; set; }    // a predefined set 
        int Capacity { get; set; }   // used for load balancing
    }

    public class FlowManager : IFlowManager
    {
        // 
        IDataService _store;
        public FlowManager(string connectionString)
        {
            _store = new DataService(connectionString);
        }
        public Task<IActivation> ActivateAsync(IActivation activation)
        {
            // An activation may request multiple items.
            // Each Item has its own initial state

            // Multiple items can be started in parallel.

            // This method must enter the activation in the database
            // enter the item instances requred with their metadata
            // return the activation which shuold now have an identity
            throw new NotImplementedException();
        }
        public Task<IOperable> GetNextAsync(IAbility ability)
        {
            // uncompleted items that qualify for the ability
            // should be traiged and the next operable returned
            // after flagging it as busy
            throw new NotImplementedException();
        }
        public Task<IOperable> UpdateAsync(IOperable operation)
        {
            // this can be used to update progress,
            // check for cancellation and
            // inform of errors or successes.
            throw new NotImplementedException();
        }
        public Task<IActivation> DeliverAsync(IActivation activation)
        {
            // update the status of an activation.
            // If it is complete, the data can be finalized and the 
            // activation slated for archive
            throw new NotImplementedException();
        }
        public async Task<List<FlowLogItem>> GetLog(int count = 100, int minSeverity = 3)
        {
            ///throw new NotImplementedException();
            await Task.CompletedTask;
            return new List<FlowLogItem>
            {
                new FlowLogItem
                { Content = "Test", EventTime = DateTime.Now, Severity = 0 }
            };
        }
        private int x = 1;
        public async Task<FlowMetrics> GetMetrics()
        {
            await Task.CompletedTask;
            x = new Random().Next(1, 5);
            return new FlowMetrics
            {
                ActivationsActive = 20 - x,
                ActivationsFailed = 3 + 2 * x,
                ActivationsRetrying = 5 - x,
                ActivationsPending = 10 + 4 * x,
                ActivationsSucceeded = 20 - x,
                ActivationsArchived = 200 + 10 * x
            };
        }
    }
}
