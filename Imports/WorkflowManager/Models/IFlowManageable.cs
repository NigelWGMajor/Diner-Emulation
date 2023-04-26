using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowManager.Models
{
    public interface IFlowManageable
    {
        long OriginId { get; }
        string Identifier { get; }
        IEnumerable<string> Items { get; } 
        IEnumerable<IEnumerable<string>> Data {  get;  } 
    }
}
