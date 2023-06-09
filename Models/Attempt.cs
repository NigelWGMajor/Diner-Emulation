﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common
{
    public class Attempt
    {
        public int DeliverableId { get; set; }
        public int MenuPlanId { get; set; }
        public int StageIndex { get; set; }
        public int Sequence { get; set; }
        public string ItemName { get; set; }
        public string ItemKind { get; set; }
        public string OperationName { get; set; }
        public string Activity { get; set; }
        public int CycleCount { get; set; }
        public string Strategy { get; set; }
        public string Message { get; set; }
        public int RetryCount { get; set; }
        public string Outcome { get; set; }
        public string Executor { get; set; }
    }
}
