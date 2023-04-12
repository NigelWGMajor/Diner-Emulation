namespace Models
{
    public enum PlanStatus
    {
        Pending,
        Opening,
        InProgress,
        Retrying,
        Completed,
        Failed
        // Waiting
    }

    public enum RecoveryStrategy
    {
        Ignore,
        Retry,
        Delay,
        RedoLast,
        Restart,
        Fail
    }
}
