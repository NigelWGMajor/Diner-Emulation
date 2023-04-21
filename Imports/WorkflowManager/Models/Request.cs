using System;
using System.Collections.Generic;
using System.Text;
using Models.Common;

namespace WorkflowManager.Models
{
    /// <summary>
    /// The request is originated by the monitor. the contained task is visible to the reciever 
    /// </summary>
    public class Request
    {
        // set by initiator
        public long RequestId { get; set; }
        public long Origin { get; set; }
        public string Initiator { get; set; }
        public string Contact { get; set; }
        public DateTime ReceivedAt { get; set; }
        public int IsActive { get; set; }
        public DateTime ReturnedAt { get; set; }
        public string Outcome { get; set; }
        public int Churn { get; set; }
    }
}

