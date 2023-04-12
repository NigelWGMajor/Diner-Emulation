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
        public static class ItemKind
    {
        public static string Main => "Main";

        public static string Side => "Side";
    }
}
