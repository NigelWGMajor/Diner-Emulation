using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowManager.Models
{
    public class Attempt : IOperable
    {
        public long PlanId { get; set; }
        public int StageIndex { get; set; }
        public int SequenceIndex { get; set; }
        public string ItemName { get; set; }
        public string ItemKind { get; set; }
        public string OperationName { get; set; }
        public int CycleCount { get; set; }
        public string Activity { get; set; }
        public string Strategy { get; set; }
        public int RetryLimit { get; set; }
        public string Message { get; set; }
        public string Outcome { get; set; }
        public string Executor { get; set; }
        public long DeliverableId { get; set; }
        public int AttemptedCount { get; set; }
        public string Data { get; set; }
        OperationState IOperable.Completion { get; set; }
    }
}
