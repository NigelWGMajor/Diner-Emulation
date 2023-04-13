using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowManager.Models
{
    /// <summary>
    /// The request is originated by the monitor. the contained task is visible to the reciever 
    /// </summary>
    public class WorkRequest
    {
        // set by initiator
        public long Id { get; set; }
        public int Origin { get; set; }
        public string Initiator { get; set; }
        public string Contact { get; set; }
        public string Request { get; set; }
        public DateTime ReceivedAt { get; set; }
        public int IsActive { get; set; }
    }
    public class WorkTask
    {
        // received by executor
        public long Id { get; set; }
        public string Executor { get; set; }
        public string Request { get; set; }
        public DateTime ReturnedAt { get; set; }
        public string Outcome { get; set; }
        public int Churn { get; set; }
    }

    public class TaskStatus
    {
        public long Id { get; set; }
        public int Origin { get; set; }
        public string Initiator { get; set; }
        public string Outcome { get; set; }
        public string Request { get; set; }
        public DateTime ReceivedAt { get; set; }
        public DateTime ReturnedAt { get; set; }
        public int Churn { get; set; }
        public int IsActive { get; set; }
    }
}

