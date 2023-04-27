using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowManager.Models
{
    public class FlowMetrics
    {
        public int ActivationsPending { get; set; }
        public int ActivationsActive { get; set; }
        public int ActivationsRetrying { get; set; }
        public int ActivationsFailed { get; set; }
        public int ActivationsSucceeded { get; set; }
        public int ActivationsArchived { get; set; }
    }
}
