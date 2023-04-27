using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowManager.Models
{
    public interface IAbility        // Qualifications for types of operation
    {
        string Name { get; set; }    // a predefined set 
        int Capacity { get; set; }   // used for load balancing
    }
    public class Ability : IAbility
    {
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }
}
