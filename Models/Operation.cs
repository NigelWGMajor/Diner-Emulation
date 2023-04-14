using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common
{
    public class Operation
    {
        public long RequestId { get; set; }
        public long MenuPlanId { get; set; }
        public long StageId { get; set; }
        public int Sequence { get; set; }
        public string ItemName { get; set; }
        public string ItemKind { get; set; }
        public string OperationName { get; set; }
        public string Activity { get; set; }
        public int CycleCount { get; set; }
        public string Strategy { get; set; }
        public string Message { get; set; }
        public int Attempts { get; set; }
        public int RetryCount { get; set; }
        public int ErrorCount { get; set; }
        public int IsComplete { get; set; }
    }
}