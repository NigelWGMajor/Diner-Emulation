using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowManager.Models
{
    public interface IActivation       // The information needed to start or end a flow.
    {
        string Name { get; }
        IEnumerable<IWorkItem> WorkItems { get; }   // the items that make up this set
        IEnumerable<string> Data { get; }   // json data
        CompletionState Completion { get; set; } // progress?
    }
    public class Activation : IActivation
    {
        public IEnumerable<IWorkItem> WorkItems { get; set ; }
        public IEnumerable<string> Data { get; set; }
        public CompletionState Completion { get; set; }
        public string Name { get; set; }
    }
}
