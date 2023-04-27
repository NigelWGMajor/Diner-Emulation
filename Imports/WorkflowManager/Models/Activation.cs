using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowManager.Models
{
    public interface IActivation       // The information needed to start or end a flow.
    {
        IEnumerable<IWorkItem> Items { get; set; }   // the items that make up this set
        IEnumerable<string> Data { get; set; }   // json data
        CompletionState Completion { get; set; } // progress?
    }
    internal class Activation : IActivation
    {
        public IEnumerable<IWorkItem> Items { get; set ; }
        public IEnumerable<string> Data { get; set; }
        public CompletionState Completion { get; set; }
    }
}
