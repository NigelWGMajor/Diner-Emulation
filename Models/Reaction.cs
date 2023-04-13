namespace Models
{
    public abstract class Reaction
    {
        public Reaction()
        {
        }

        public Reaction(string message = "", int retryCount = 3)
        {
            _message = message;
            RetryCount = retryCount;
        }

        private string _message;

        public string Message
        {
            get
            {
                if (string.IsNullOrEmpty(_message))
                    return _defaults[(int) Strategy];
                return _message;
            }
            set
            {
                _message = value;
            }
        }

        public virtual RecoveryStrategy Strategy => RecoveryStrategy.Ignore;

        // This sequence must exactly match the RecoveryStrategy enumeration
        private readonly string[] _defaults =
                {
                    "Activity is being skipped", // Ignore
                    "Activity is being retried", // Retry
                    "Activity will be retried after a delay", // Delay
                    "Reverting to the prior activity", // RedoLast
                    "Restarting from the beginning", // Restart
                    "Unable to complete" // Fail
                };

        protected int RetryCount { get; set; }
    }

    public class ReactIgnore : Reaction
    {
        public ReactIgnore(string message = "", int retryCount = 0) :
            base(message, retryCount)
        {
        }

        public override RecoveryStrategy Strategy => RecoveryStrategy.Ignore;
    }

    public class ReactRetry : Reaction
    {
        public ReactRetry(string message = "", int retryCount = 5) :
            base(message, retryCount)
        {
        }

        public override RecoveryStrategy Strategy => RecoveryStrategy.Retry;
    }

    public class ReactDelay : Reaction
    {
        public ReactDelay(string message = "", int retryCount = 2) :
            base(message, retryCount)
        {
        }

        public override RecoveryStrategy Strategy => RecoveryStrategy.Delay;
    }

    public class ReactRedoLast : Reaction
    {
        public ReactRedoLast(string message = "", int retryCount = 2) :
            base(message, retryCount)
        {
        }

        public override RecoveryStrategy Strategy => RecoveryStrategy.RedoLast;
    }

    public class ReactRestart : Reaction 
    {
        public ReactRestart(string message = "", int retryCount = 2) :
            base(message, retryCount)
        {
        }

        public override RecoveryStrategy Strategy => RecoveryStrategy.Restart;
    }

    public class ReactFail : Reaction
    {
        public ReactFail(string message = "", int retryCount = 0) :
            base(message, retryCount)
        {
        }

        public override RecoveryStrategy Strategy => RecoveryStrategy.Fail;
    }
}
