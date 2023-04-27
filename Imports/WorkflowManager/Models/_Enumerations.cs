using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowManager.Models
{
    [Flags]
    public enum CompletionState
    {   // need to be simple but useful
        Pending = 0,     // waiting for service
        Active = 1,      // is started, actively being worked 
        Retrying = 2,    // had errors but not fatal
        Cancelled = 32,  // future?
        Failed = 64,     // completed but unsuccessful
        Succeeded = 128, // Completed, done
        Archived = 256   // retired to storage
    }
}
