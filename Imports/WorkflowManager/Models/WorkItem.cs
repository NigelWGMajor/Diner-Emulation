using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowManager.Models
{
    public interface IWorkItem           // identifies a single thing that has a single defined workflow
    {
        string Name { get; set; }    //  important, because this defines the workflow rules
        string Data { get; set; }   // json payload
    }
    public class WorkItem : IWorkItem
    {
        public string Name { get; set; }
        public string Data { get; set; }
    }
}
