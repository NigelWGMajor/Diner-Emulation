using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowManager.Models
{
    public  class Trial
    {
        public long TrialId { get; set; }
        public string Strategy { get; set; }
        public string Message { get; set; }
        public int RetryLimit { get; set; }
        public long StageId { get; set; }
    }
}
