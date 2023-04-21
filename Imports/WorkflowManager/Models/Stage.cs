using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace WorkflowManager.Models
{
    public class Stage
    {
        public string Name { get; set; }
        public string Activity { get; set; }
        public int CycleCount { get; set; }
        public long PlanId { get; set; }
        public int SequenceIndex { get; set; }
    }
}
