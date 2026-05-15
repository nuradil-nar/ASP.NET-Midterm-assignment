namespace TaskTrackerAPI.Models;

public enum SeverityLevel
{
    Low,
    Medium,
    High,
    Critical
}

public class BugReportTask : BaseTask
{
    public SeverityLevel SeverityLevel { get; set; }

    public BugReportTask() : base() { }

    public BugReportTask(Guid id, DateTime createdAt) : base(id, createdAt) { }
}
