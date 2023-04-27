using System;
using WorkflowManager.Models;
using System.Threading.Tasks;
using System.Linq;
using WorkflowManager.Services;
using System.Collections;
using System.Collections.Generic;

namespace WorkflowManager
{
    public interface IFlowManager
    {
        Task<IActivation> ActivateAsync(IActivation activation); // Consumer Activates a request
        Task<IOperable> ProceedAsync(IAbility ability = null);   // Provider gets operation based on ability
        Task<IOperable> UpdateAsync(IOperable operation);        // Update final or intermediate status or detect cancellation
        Task<IActivation> ProgressAsync(IActivation activation);  // Consumer retrieves final state/status
        Task<List<FlowLogItem>> GetLog(int count = 100, int minSeverity = 3);  // Retrieve recent events 
        Task<FlowMetrics> GetMetrics(); // Get the prevailing state of the activations
    }
    
    public class FlowManager : IFlowManager
    {
        // 
        IDataService _store;
        public FlowManager(string connectionString)
        {
            _store = new DataService(connectionString);
        }
        public async Task<IActivation> ActivateAsync(IActivation activation)
        {
            // An activation may request multiple items.
            // Each Item has its own initial state

            // Multiple items can be started in parallel.

            // This method must enter the activation in the database
            // enter the item instances required with their metadata
            // return the activation which should now have an identity
            await Task.CompletedTask;
            return default(Activation);
        }
        public async Task<IOperable> ProceedAsync(IAbility ability)
        {
            // uncompleted items that qualify for the ability
            // should be traiged and the next operable returned
            // after flagging it as busy
            await Task.CompletedTask;
            return default(Operable);
        }
        public async Task<IOperable> UpdateAsync(IOperable operation)
        {
            // this can be used to update progress,
            // check for cancellation and
            // inform of errors or successes.
            await Task.CompletedTask;
            return operation;
        }
        public async Task<IActivation> DeliverAsync(IActivation activation)
        {
            // update the status of an activation.
            // If it is complete, the data can be finalized and the 
            // activation slated for archive
            await Task.CompletedTask;
            return activation;
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
