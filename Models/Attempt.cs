using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common
{
    public class Attempt
    {
        public bool Succeeded { get; set; }
        public string Operation { get; set; }
        public string Strategy { get; set; }
        public int RetryCount { get; set; }
        public string Message { get; set; }
    }
}
