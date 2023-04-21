using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowManager.Models
{
    public class Plan
    {
        public int PlanId { get; set; }
        public string Status { get; set; }
        public string ItemName { get; set; }
        public string ItemKind { get; set; }
        public string Description { get; set; }
        public int Availiablility { get; set; }
    }
}
