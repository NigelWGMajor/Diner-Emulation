using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowManager.Models
{
    public class FlowLogItem
    {
        public string Content { get; set; }
        /// <summary>
        /// 0 = Informational/blue 1 = Good/green 2 = Warn/yellow 3 = Error/Red
        /// </summary>
        public int Severity { get; set; }
        public DateTime EventTime { get; set; }
    }
}
