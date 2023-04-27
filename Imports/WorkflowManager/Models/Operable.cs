using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowManager.Models
{
    public interface IOperable       
    {
        string OperationName { get; set; } 
        CompletionState Completion { get; set; } 
        string Data { get; set; }
    }
    public class Operable : IOperable
    {
        public string OperationName { get; set; }
        public string Data { get; set; }
        public CompletionState Completion { get; set; }
    }
}
