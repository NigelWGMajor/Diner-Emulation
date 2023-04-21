using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Emulator.Models;
using WorkflowManager.Models;

namespace WorkflowManager.Services
{
    public interface IDataService
    {
        Task<long> AddRequest(Request request);
        Task AddDeliverable(Deliverable deliverable);

        Task<Trial> GetNextTrial();

        Task NotifyOutcome(Trial trial);

        Task<Deliverable> ReceiveDeliverable(long deliverableId);
    }
    public class DataService : IDataService
    {
        public Task AddDeliverable(Deliverable deliverable)
        {
            throw new NotImplementedException();
        }

        public Task<long> AddRequest(Request request)
        {
            throw new NotImplementedException();
        }

        public Task<Trial> GetNextTrial()
        {
            throw new NotImplementedException();
        }

        public Task NotifyOutcome(Trial trial)
        {
            throw new NotImplementedException();
        }

        public Task<Deliverable> ReceiveDeliverable(long deliverableId)
        {
            throw new NotImplementedException();
        }
    }





}
